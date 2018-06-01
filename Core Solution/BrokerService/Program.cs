using System;
using System.IO;
using System.Threading;
using Fclp;
using Repository;
using Serilog;

namespace BrokerService
{
    internal static class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            ProcessArguments(args);
            AppSettings.Logger = ConfigureLogging();
            AppSettings.Logger.Information(
                $"Broker Service has started and running in {(AppSettings.IsProduction ? "PROD" : "DEV")}");

            AppSettings.Logger.Information("Checking database configuration...");
            DatabaseUtils.ConfigureDatabase();
            AppSettings.Logger.Information(
                "Database configuration complete and is at /var/www/DeathRaffle/Data/DeathRaffle.sqlite");

            var stellarClient = new StellarClient();

            stellarClient.StartWalletWatcher();

            QuitEvent.WaitOne();

            Log.CloseAndFlush();
        }

        private static void ProcessArguments(string[] args)
        {
            var parser = new FluentCommandLineParser();
            var environment = "";

            parser.Setup<string>('e', "environment") // define the short and long option name
                .Required()
                .Callback(arg => environment = arg.ToLower())
                .WithDescription("Specify whether this is a development (dev) or production (prod) environment");

            parser.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));

            var result = parser.Parse(args);
            var checkEnv = environment == "dev" || environment == "prod";

            if (!result.HasErrors && checkEnv)
            {
                if (environment == "prod")
                {
                    AppSettings.WalletPublicKey = "GC4SFDYTS5IF7UJOJRSPAUW4MOTPTF67T67Q2YEH7NCKX4WEAYCLA4D4";
                    AppSettings.WalletPrivateKey = "SDNJ3JOBZO5O3ATNDOBZHAYG7H3DJE6Z7OGMWEADTLNZXCIWPW6WE7OR";
                    AppSettings.IsProduction = true;
                }
                else
                {
                    AppSettings.WalletPublicKey = "GC4SFDYTS5IF7UJOJRSPAUW4MOTPTF67T67Q2YEH7NCKX4WEAYCLA4D4";
                    AppSettings.WalletPrivateKey = "SDNJ3JOBZO5O3ATNDOBZHAYG7H3DJE6Z7OGMWEADTLNZXCIWPW6WE7OR";
                    AppSettings.IsProduction = false;
                }
               
            }
            else
            {
                Console.WriteLine($"Error: {result.ErrorText}");
                Environment.Exit(0);
            }
        }

        private static ILogger ConfigureLogging()
        {
            var logDir = Path.Combine("/var/www/DeathRaffle/Logs");

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.RollingFile(Path.Combine(logDir, "Broker_Service.log"))
                .CreateLogger();

            Log.Information($"Log file created at {Path.Combine(logDir, "Broker_Service.log")}");

            return Log.Logger;
        }
    }
}