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

        Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(string? searchText, Guid? categoryId, int page, int pageSize);
    }
}
