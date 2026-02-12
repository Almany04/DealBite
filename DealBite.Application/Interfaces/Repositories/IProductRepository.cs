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
        Task<IEnumerable<Product>> GetByIdWithDetailsAsync(Guid productId);
    }
}
