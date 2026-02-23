using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Repositories
{
    public class AppUserRepository : GenericRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AppUser?> GetByIdentityUserIdAsync(Guid identityUserId)
        {
            return await _context.AppUsers
                 .FirstOrDefaultAsync(p => p.IdentityUserId == identityUserId);
        }
    }
}
