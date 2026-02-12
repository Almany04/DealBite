using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DealBite.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
           return await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.Prices)
                .ThenInclude(pp=>pp.Store)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithDetailsAsync(Guid Id)
        {
            return await _context.Products
                 .AsNoTracking()
                 .Include(p => p.Category)
                 .Include(p => p.Prices)
                 .ThenInclude(pp => pp.Store)
                 .FirstOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
