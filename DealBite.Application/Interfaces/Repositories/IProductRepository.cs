using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
        Task<Product?> GetByIdWithDetailsAsync(Guid productId);
        Task<List<Product>> GetByStoreIdAsync(Guid storeId, bool onlyActive=false);
        Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(string? searchText, Guid? categoryId, int page, int pageSize);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetOnSaleAsync(string? searchText, Guid? categoryId, int page, int pageSize);

    }
}
