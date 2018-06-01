using System;

namespace Repository.HelperModels
{
    public class AdminDashboardPools
    {
        public long Id { get; set; }
        public int TicketCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime GameStarted { get; set; }
        public DateTime GameEnded { get; set; }
    }
}