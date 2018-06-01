using Repository.Models;

namespace Web.Controllers.Models
{
    public class TicketSearchResult
    {
        public long TicketId { get; set; }
        public long PoolId { get; set; }
        public string PlayerAddress { get; set; }
        public Celebrity Celebrity { get; set; }
    }
}