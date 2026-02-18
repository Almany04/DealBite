using DealBite.Application.Auth.DTO;
using DealBite.Application.Auth.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
    }
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IAuthService _auth;
        private readonly ITokenService _token;

        public RegisterCommandHandler(IAuthService auth, ITokenService token)
        {
            _auth = auth;
            _token = token;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result= await _auth.RegisterAsync(request.Email, request.Password, request.DisplayName);
            if (!result.Success)
            {
                throw new Exception(result.Error);
            }
            var token = _token.GenerateToken(result.UserId!.Value, result.Email!);

            return new AuthResponse
            {
                Token = token,
                Email = result.Email!,
                DisplayName=request.DisplayName
            };
        }
    }
}
