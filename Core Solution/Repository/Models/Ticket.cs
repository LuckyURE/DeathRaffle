using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    [Table("Tickets")]
    public class Ticket
    {
        public long Id { get; set; }
        public string PlayerAddress { get; set; }
        public long CelebrityId { get; set; }
        public long PoolId { get; set; }
    }
}