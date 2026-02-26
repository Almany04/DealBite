using DealBite.Application.Auth.DTO;
using DealBite.Application.Auth.Interfaces;
using DealBite.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _auth;
        private readonly ITokenService _token;

        public LoginCommandHandler(IAuthService auth, ITokenService token)
        {
            _auth = auth;
            _token = token;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        { 
            var result = await _auth.LoginAsync(request.Email, request.Password);
            if (!result.Success)
            {
                throw new Exception(result.Error);
            }

            var token = _token.GenerateToken(result.UserId!.Value, result.Email!);

            return new AuthResponse
            {
                Token = token,
                Email = result.Email!,
                DisplayName = result.DisplayName!
            };
        }
    }
}
