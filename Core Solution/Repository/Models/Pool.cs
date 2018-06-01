using System;
using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    [Table("Pools")]
    public class Pool
    {
        public long Id { get; set; }
        public long WinningTicket { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? LockDate { get; set; }
    }
}