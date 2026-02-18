using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Auth.DTO
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        
    }
}
