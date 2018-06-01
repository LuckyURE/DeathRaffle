using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    [Table("Wallets")]
    public class Wallet
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string LastToken { get; set; }
    }
}