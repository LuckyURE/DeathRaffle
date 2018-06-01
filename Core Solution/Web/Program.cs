using System;
using System.IO;
using System.Net;
using Fclp;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Repository;
using Serilog;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseUtils.ConfigureDatabase();
            AppSettings.Logger = ConfigureLogging();
            ProcessArguments(args);
            AppSettings.Logger.Information(
                $"Web has started and running in {(AppSettings.IsProduction ? "PROD" : "DEV")}");
            BuildWebHost().Run();
        }

        public static IWebHost BuildWebHost() =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5002);
                })
                .Build();

        private static ILogger ConfigureLogging()
        {
            var logDir = Path.Combine("/var/www/DeathRaffle/Logs");

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(logDir, "Web_Service.log"))
                .CreateLogger();

            Log.Information($"Log file created at {Path.Combine(logDir, "Web_Service.log")}");

            return Log.Logger;
        }
        
        private static void ProcessArguments(string[] args)
        {
            var parser = new FluentCommandLineParser();
            var environment = "";

            parser.Setup<string>('e', "environment") // define the short and long option name
                .Required()
                .Callback(arg => environment = arg.ToLower())
                .WithDescription("Specify whether this is a development (dev) or production (prod) environment");

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
                AppSettings.Logger.Error("There was an error starting the application.");
                Environment.Exit(0);
            }
        }
    }
}