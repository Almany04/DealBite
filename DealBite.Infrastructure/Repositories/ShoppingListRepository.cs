using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Repositories
{
    public class ShoppingListRepository : GenericRepository<ShoppingList>, IShoppingListRepository
    {
        public ShoppingListRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ShoppingList?> GetByIdWithItemsAsync(Guid id)
        {
            return await _context.ShoppingLists
                .AsNoTracking()
                .Include(sh => sh.ShoppingListItems)
                    .ThenInclude(s => s.Product)
                .Include(sh => sh.ShoppingListItems)
                    .ThenInclude(s => s.Store)
                .FirstOrDefaultAsync(p => p.Id == id);  
        }

        public async Task<List<ShoppingList>> GetByUserIdAsync(Guid UserId)
        {
            return await _context.ShoppingLists
                .AsNoTracking()
                .Where(p => p.UserId == UserId)
                .ToListAsync();
        }

        
    }
}
