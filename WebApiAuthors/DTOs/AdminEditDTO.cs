using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class AdminEditDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
