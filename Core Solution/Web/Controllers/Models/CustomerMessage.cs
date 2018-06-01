using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Models
{
    public class CustomerMessage
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }
    }
}