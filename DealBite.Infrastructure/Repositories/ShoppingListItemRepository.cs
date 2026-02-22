using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Repositories
{
    public class ShoppingListItemRepository : GenericRepository<ShoppingListItem>, IShoppingListItemRepository
    {
        public ShoppingListItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
