using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    public class User
    {
        [Key] 
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}