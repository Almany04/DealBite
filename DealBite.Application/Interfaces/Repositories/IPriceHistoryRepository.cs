using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface IPriceHistoryRepository:IGenericRepository<PriceHistory>
    {
        Task<IEnumerable<PriceHistory>> GetByProductIdAsync(Guid productId);
    }
}
