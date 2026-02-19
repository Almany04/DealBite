using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface IShoppingListRepository:IGenericRepository<ShoppingList>
    {
        Task<List<ShoppingList>> GetByUserIdAsync(Guid UserId);
        Task<ShoppingList?> GetByIdWithItemsAsync(Guid id);
    }
}
