using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace Repository.Models
{
    [Table("Celebrities")]
    public class Celebrity
    {
        public long Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Suffix { get; set; }
        [Required]
        public int BirthYear { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public bool Dead { get; set; }

        [Computed]
        public int Age => DateTime.Now.Year - BirthYear;
    }
}