
using System.ComponentModel.DataAnnotations;

namespace DealBite.Application.Auth.DTO
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }
        [Required]
        public required string Password { get; init; }
    }
}
