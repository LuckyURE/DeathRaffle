using System;

namespace Repository.HelperModels
{
    public class DashboardPool
    {
        public long PoolId { get; set; }
        public int TicketCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime GameStarted { get; set; }
    }
}