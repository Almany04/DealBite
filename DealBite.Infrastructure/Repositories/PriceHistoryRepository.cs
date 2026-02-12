using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Repositories
{
    public class PriceHistoryRepository : GenericRepository<PriceHistory>, IPriceHistoryRepository
    {
        public PriceHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PriceHistory>> GetByProductIdAsync(Guid productId)
        {
            return await _context.PriceHistories
                .AsNoTracking()
                .Where(ph => ph.ProductId == productId)
                .Include(ph => ph.Store)
                .OrderBy(ph => ph.RecordedAt)
                .ToListAsync();
        }
    }
}
