using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                .AsNoTracking()
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

        public async Task<List<Product>> GetByStoreIdAsync(Guid storeId, bool onlyActive = false)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            query = query.Where(p => p.Prices.Any(price => price.StoreId == storeId));

            if (onlyActive)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                query = query.Where(p => p.Prices.Any(price =>
                    price.StoreId == storeId &&
                    price.ValidTo >= today));
            }

            return await query
                .Include(p => p.Category)
                .Include(p => p.Prices)
                .ThenInclude(pp => pp.Store)
                .ToListAsync();
        }

        public async Task<ProductPrice?> GetEstimatedPriceMinimumWithDetailsAsync(Guid productId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.ProductPrices
                .AsNoTracking()
                .Where(p => p.ProductId == productId && p.ValidTo >= today)
                .Include(p => p.Store)
                .OrderBy(p => p.Price.Amount)
                .FirstOrDefaultAsync();

        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetOnSaleAsync(string? searchText, Guid? categoryId, int page, int pageSize)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => EF.Functions.ILike(p.NormalizedName, $"%{searchText}%"));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            query = query.Where(p => p.Prices.Any(price =>
            price.IsOnSale == true &&
            price.ValidFrom <= today &&
            price.ValidTo >= today
            ));

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(p => p.Category)
                .Include(p => p.Prices).ThenInclude(pp => pp.Store)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<ProductPrice>> GetOnSaleProductPricesAsync(Guid? storeId, ProductSegment segment)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var query = _context.ProductPrices
                .AsNoTracking()
                .Where(s => s.IsOnSale && s.ValidFrom <= today && s.ValidTo >= today)
                .Where(p => p.Product!.Segment == segment);

            if (storeId.HasValue)
            {
                query = query.Where(p => p.StoreId == storeId.Value);
            }

            return await query
                .Include(p => p.Product).ThenInclude(pr => pr!.Category)
                .Include(p => p.Store)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<ProductPrice>> GetProductsWithPricesAsync(List<Guid> productIds)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.ProductPrices
                 .Include(p=>p.Store)
                 .Where(p => productIds.Contains(p.ProductId) && p.ValidTo >= today)
                 .ToListAsync();
                
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(string? searchText, Guid? categoryId, int page, int pageSize)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => EF.Functions.ILike(p.NormalizedName, $"%{searchText}%"));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(p => p.Category)
                .Include(p => p.Prices).ThenInclude(pp => pp.Store)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
