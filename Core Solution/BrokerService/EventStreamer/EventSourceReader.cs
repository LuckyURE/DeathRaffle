using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BrokerService.EventStreamer
{
    public class EventSourceReader : IDisposable
    {
        private const string DefaultEventType = "message";

        public delegate void MessageReceivedHandler(object sender, EventSourceMessageEventArgs e);

        public delegate void DisconnectEventHandler(object sender, DisconnectEventArgs e);

        private readonly HttpClient _httpClient = new HttpClient();
        private Stream _stream;
        private readonly Uri _uri;

        private volatile bool _isDisposed;
        public bool IsDisposed => _isDisposed;

        private volatile bool _isReading;
        private readonly object _startLock = new object();

        private int _reconnectDelay = 3000;
        private string _lastEventId = string.Empty;

        public event MessageReceivedHandler MessageReceived;
        public event DisconnectEventHandler Disconnected;

        /// <summary>
        /// An instance of EventSourceReader
        /// </summary>
        /// <param name="url">URL to listen from</param>
        public EventSourceReader(Uri url)
        {
            _uri = url;
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/event-stream");
        }


        /// <summary>
        /// Returns instantly and starts listening
        /// </summary>
        /// <returns>current instance</returns>
        /// <exception cref="ObjectDisposedException">Dispose() has been called</exception>
        public EventSourceReader Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("EventSourceReader");
            }

            lock (_startLock)
            {
                if (_isReading) return this;
                _isReading = true;
                // Only start a new one if one isn't already running
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                ReaderAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            return this;
        }


        /// <inheritdoc />
        /// <summary>
        /// Stop and dispose of the EventSourceReader
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            _stream?.Dispose();
            _httpClient.CancelPendingRequests();
            _httpClient.Dispose();
        }


        private async Task ReaderAsync()
        {
            try
            {
                if (string.Empty != _lastEventId)
                {
                    if (_httpClient.DefaultRequestHeaders.Contains("Last-Event-Id"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Last-Event-Id");
                    }

                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Last-Event-Id", _lastEventId);
                }

                using (var response = await _httpClient.GetAsync(_uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    if (response.Headers.TryGetValues("content-type", out var ctypes) ||
                        ctypes?.Contains("text/event-stream") == false)
                    {
                        throw new ArgumentException("Specified URI does not return server-sent events");
                    }

                    _stream = await response.Content.ReadAsStreamAsync();
                    using (var sr = new StreamReader(_stream))
                    {
                        var evt = DefaultEventType;
                        var id = string.Empty;
                        var data = new StringBuilder(string.Empty);

                        while (!sr.EndOfStream)
                        {
                            var line = await sr.ReadLineAsync();
                            if (line == string.Empty)
                            {
                                // double newline, dispatch message and reset for next
                                MessageReceived?.Invoke(this,
                                    new EventSourceMessageEventArgs(data.ToString().Trim(), evt, id));
                                data.Clear();
                                id = string.Empty;
                                evt = DefaultEventType;
                                continue;
                            }

                            if (line.First() == ':')
                            {
                                // Ignore comments
                                continue;
                            }

                            var dataIndex = line.IndexOf(':');
                            string field;
                            if (dataIndex == -1)
                            {
                                dataIndex = line.Length;
                                field = line;
                            }
                            else
                            {
                                field = line.Substring(0, dataIndex);
                                dataIndex += 1;
                            }

                            var value = line.Substring(dataIndex).Trim();

                            switch (field)
                            {
                                case "event":
                                    // Set event type
                                    evt = value;
                                    break;
                                case "data":
                                    // Append a line to data using a single \n as EOL
                                    data.Append($"{value}\n");
                                    break;
                                case "retry":
                                    // Set reconnect delay for next disconnect
                                    int.TryParse(value, out _reconnectDelay);
                                    break;
                                case "id":
                                    // Set ID
                                    _lastEventId = value;
                                    id = _lastEventId;
                                    break;
                                default:
                                    AppSettings.Logger.Error("An event type was not accounted for.");
                                    break;
                            }
                        }
                    }

                    Disconnect(null);
                }
            }
            catch (Exception ex)
            {
                Disconnect(ex);
            }
        }

        private void Disconnect(Exception ex)
        {
            _isReading = false;
            Disconnected?.Invoke(this, new DisconnectEventArgs(_reconnectDelay, ex));
        }
    }
}