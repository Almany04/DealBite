
using System.ComponentModel.DataAnnotations;

namespace DealBite.Application.Auth.DTO
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
        [Required]
        [MaxLength(50)]
        public required string DisplayName { get; set; }
    }
}
