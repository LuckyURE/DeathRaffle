using Serilog;

namespace BrokerService
{
    public static class AppSettings
    {
        public static bool IsProduction { get; set; }
        public static ILogger Logger { get; set; }
        public static string WalletPublicKey { get; set; }
        public static string WalletPrivateKey { get; set; }
    }
}