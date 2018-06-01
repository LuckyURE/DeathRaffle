using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrokerService.EventStreamer;
using BrokerService.Models;
using Repository.Models;
using Repository.Repositories;
using RestSharp;
using stellar_dotnetcore_sdk;
using stellar_dotnetcore_sdk.responses;
using stellar_dotnetcore_sdk.responses.operations;

namespace BrokerService
{
    public class StellarClient
    {
        private KeyPair _keyPair;
        private readonly Server _server;
        private Wallet _wallet;
        private readonly WalletsRepository _walletRepository;
        private readonly PoolRepository _poolRepository;
        private readonly TicketRepository _ticketRepository;
        private readonly AppInfoRepository _appInfoRepository;
        private string _url;

        public StellarClient()
        {
            _walletRepository = new WalletsRepository();
            _poolRepository = new PoolRepository();
            _ticketRepository = new TicketRepository();
            _appInfoRepository = new AppInfoRepository();

            if (AppSettings.IsProduction)
            {
                AppSettings.Logger.Information("Connected to the public Stellar network.");
                Network.UsePublicNetwork();
                _server = new Server("https://horizon.stellar.org");
            }
            else
            {
                AppSettings.Logger.Information("Connected to the TEST Stellar network.");
                Network.UseTestNetwork();
                _server = new Server("https://horizon-testnet.stellar.org");
            }

            GetWallet();
            
            _url = $"https://horizon.stellar.org/accounts/{_keyPair.AccountId}/payments";
        }

        public void StartWalletWatcher()
        {
            var lastToken = _wallet.LastToken;

            if (string.IsNullOrWhiteSpace(lastToken))
            {
                _url += "?cursor=now";
            }
            else
            {
                _url += $"?cursor={lastToken}";
            }

            AppSettings.Logger.Information($"Starting watcher with cursor at {lastToken}");

            Console.Write("Waiting for incoming payments...\n");
            var eventStream = new EventSourceReader(new Uri(_url)).Start();

            eventStream.MessageReceived += (sender, response) => ShowOperationResponse(response);
            eventStream.Disconnected += async (sender, response) =>
            {
                if (!response.Exception.Message.Contains("GATEWAY_TIMEOUT"))
                {
                    Console.WriteLine($"Retry: {response.ReconnectDelay} - Error: {response.Exception}");
                }

                await Task.Delay(response.ReconnectDelay);
                eventStream.Start(); // Reconnect to the same URL
            };
        }

        private void ShowOperationResponse(EventSourceMessageEventArgs op)
        {
            if (op.Event == "open") return;
            var or = JsonSingleton.GetInstance<OperationResponse>(op.Message);

            SavePagingToken(or.PagingToken);

            if (or.GetType() != typeof(PaymentOperationResponse)) return;
            var payment = (PaymentOperationResponse) or;
            if (payment.From.AccountId == _keyPair.AccountId) return;

            AppSettings.Logger.Information(
                $@"Transaction received: Id: {or.Id}, Source: {or.SourceAccount.AccountId}, Type: {or.Type}");

            // Need to make sure the payment was at least $10 worth of XRP
            // If it isn't enough, refund the money minus a small transaction fee.
            var paymentAmount = decimal.Parse(payment.Amount);
            if (GetMinimumLumensRequired() > paymentAmount)
            {
                AppSettings.Logger.Information(
                    $"Received underfunded pament of {paymentAmount} lumens from {payment.From.AccountId}! Refunding...");

                SendPayment(
                    payment.From.AccountId,
                    (paymentAmount - 0.0000200m).ToString("0.0000000"),
                    "Err: http://bit.ly/2FW9r0m");

                return;
            }

            AppSettings.Logger.Information("Connecting to the database to get the availale pool...");
            var poolId = _poolRepository.GetAvailablePoolByPayer(payment.From.AccountId);

            if (poolId == 0)
            {
                poolId = _poolRepository.Add(new Pool
                {
                    CreateDate = DateTime.Now
                });

                AppSettings.Logger.Information($"Created new Pool {poolId}.");
            }

            var celebId = _poolRepository.GetAvailablePoolCelebrity(poolId);

            var ticketId = _ticketRepository.Add(new Ticket
            {
                PlayerAddress = payment.From.AccountId,
                CelebrityId = celebId,
                PoolId = poolId
            });

            AppSettings.Logger.Information(
                "Created a new Ticket: \n " +
                $"Ticket ID: {ticketId} \n " +
                $"Player Address: {payment.From.AccountId} \n" +
                $"Celebrity ID: {celebId} \n" +
                $"Pool ID: {poolId}");

            SendPayment(
                or.SourceAccount.AccountId,
                "0.0000001",
                $"Raffle Ticket Id: {ticketId}");
        }

        private void SavePagingToken(string token)
        {
            _wallet.LastToken = token;
            _walletRepository.SaveLastToken(_wallet);
        }

        private decimal GetMinimumLumensRequired()
        {
            var client = new RestClient("https://api.coinmarketcap.com/v1/ticker/stellar/?convert=usd");
            var request = new RestRequest(Method.GET);

            try
            {
                var response = client.Execute<List<TickerReponse>>(request).Data.First();

                AppSettings.Logger.Information($"Current XLM Price: {response.PriceUsd}");

                var lumensRequired = 10.00m / decimal.Parse(response.PriceUsd);

                _appInfoRepository.UpdateLumensRequired(lumensRequired);

                return lumensRequired;
            }
            catch (Exception)
            {
                AppSettings.Logger.Error("Unable to connect to CoinMarketCap API!");
                return _appInfoRepository.GetAppInfo().LumensRequired;
            }
        }

        private void GetWallet()
        {
            // Second TEST Wallet ID:    GC4SFDYTS5IF7UJOJRSPAUW4MOTPTF67T67Q2YEH7NCKX4WEAYCLA4D4
            // Second TEST Wallet Seed:  SDNJ3JOBZO5O3ATNDOBZHAYG7H3DJE6Z7OGMWEADTLNZXCIWPW6WE7OR

            // First TEST Wallet Id:     GCIJAGYNWPOKYBNYEG6PUPNSNTED3TTDRV6BXUHPXQV3IPDUVOEA5TJP
            // First TEST Wallet Seed:   SDE6NV3POKTX6APDVB7LMTOXPFW6EEM53CGAFCMTJWNBKCHABFAGOT7K    

            _keyPair = KeyPair.FromSecretSeed(AppSettings.WalletPrivateKey);
            _wallet = _walletRepository.GetWallet(AppSettings.WalletPublicKey)
                      ?? _walletRepository.Add(new Wallet()
                      {
                          Address = AppSettings.WalletPublicKey,
                          LastToken = ""
                      });
        }

        private async void SendPayment(string accountId, string amount, string memo)
        {
            var toAccount = KeyPair.FromAccountId(accountId);
            var accountInfo = await _server.Accounts.Account(_keyPair);
            var fromAccount = new Account(_keyPair, accountInfo.SequenceNumber);

            if (toAccount.AccountId == fromAccount.KeyPair.AccountId) return;

            var transaction = new Transaction.Builder(fromAccount)
                .AddOperation(new PaymentOperation.Builder(
                    toAccount,
                    new AssetTypeNative(),
                    amount).Build())
                .AddMemo(Memo.Text(memo))
                .Build();
            transaction.Sign(_keyPair);

            try
            {
                var response = await _server.SubmitTransaction(transaction);
                AppSettings.Logger.Information($"Payment was sent successfully. Ledger: {response.Ledger}");
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Payment was not sent successfully. Error was: \n {e.Message}");
            }
        }
    }
}