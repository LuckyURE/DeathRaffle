using System;

namespace Web.Controllers.Models
{
    public class PoolDashboardResult
    {
        public long PoolId { get; set; }
        public int TicketCount { get; set; }
        public DateTime CreateDate { get; set; }
        public bool GameStarted { get; set; }
        public int LumensRequired { get; set; }
    }
}