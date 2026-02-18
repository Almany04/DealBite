using DealBite.Application.Auth;
using DealBite.Application.Auth.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DealBite.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<JwtSettings> _jwt;

        public TokenService(IOptions<JwtSettings> jwt)
        {
            _jwt = jwt;
        }

        public string GenerateToken(Guid userId, string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Value.Issuer,
                audience: _jwt.Value.Audience,
                claims: claims,
                expires:DateTime.UtcNow.AddMinutes(_jwt.Value.ExpiryMinutes),
                signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
