using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface IAppUserRepository:IGenericRepository<AppUser>
    {
        Task<AppUser?> GetByIdentityUserIdAsync(string identityUserId);
    }
}
