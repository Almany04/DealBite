using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace DealBite.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetAllWithSubCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId==null)
                .Include(c => c.SubCategories)
                .ToListAsync();
        }

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            return await _context.Categories.
                AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }
    }
}
