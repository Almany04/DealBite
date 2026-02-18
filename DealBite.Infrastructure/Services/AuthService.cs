using DealBite.Application.Auth.Interfaces;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Identity;
using DealBite.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DealBite.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationDbUser> _manager;
        private readonly ApplicationDbContext _context;
        private readonly IGenericRepository<AppUser> _appuserRepository;

        public AuthService(UserManager<ApplicationDbUser> manager, ApplicationDbContext context, IGenericRepository<AppUser> appuserRepository)
        {
            _manager = manager;
            _context = context;
            _appuserRepository = appuserRepository;
        }

        public async Task<(bool Success, string? Error, Guid? UserId, string? Email)> LoginAsync(string email, string password)
        {
            var findByEmail =await _manager.FindByEmailAsync(email);

            if (findByEmail == null)
            {
                return (false, "Hibás email vagy jelszó", null, null);
            }
            var checkPassword = await _manager.CheckPasswordAsync(findByEmail ,password);

            if (!checkPassword)
            {
                return (false, "Hibás email vagy jelszó", null, null);
            }

            return (true, null, findByEmail.Id, findByEmail.Email);
        }

        public async Task<(bool Success, string? Error, Guid? UserId, string? Email)> RegisterAsync(string email, string password, string displayName)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync<(bool Success, string? Error, Guid? UserId, string? Email)>(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var identityUser = new ApplicationDbUser
                    {
                        Email = email,
                        UserName = email
                    };

                    var result = await _manager.CreateAsync(identityUser, password);
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                    }

                    var appUser = new AppUser
                    {
                        IdentityUserId = identityUser.Id.ToString(),
                        Email = email,
                        DisplayName = displayName,
                        LastLoginAt = DateTimeOffset.UtcNow
                    };

                    await _appuserRepository.AddAsync(appUser);

                    await transaction.CommitAsync();

                    return (true, null, identityUser.Id, email);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, ex.Message, null, null);
                }
            });
        }
    }
}
