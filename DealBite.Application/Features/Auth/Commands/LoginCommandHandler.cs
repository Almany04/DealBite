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
        private readonly IAppUserRepository _appUserRepository;

        public LoginCommandHandler(IAuthService auth, ITokenService token,IAppUserRepository appUserRepository)
        {
            _auth = auth;
            _token = token;
            _appUserRepository = appUserRepository;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        { 
            var result = await _auth.LoginAsync(request.Email, request.Password);
            if (!result.Success)
            {
                throw new Exception(result.Error);
            }

            var token = _token.GenerateToken(result.UserId!.Value, result.Email!);

            var appUser = await _appUserRepository.GetByIdentityUserIdAsync(result.UserId!.Value);
            if (appUser == null)
            {
                throw new Exception("Felhasználó nem található");
            }

            return new AuthResponse
            {
                Token = token,
                Email = result.Email!,
                DisplayName = appUser.DisplayName
            };
        }
    }
}
