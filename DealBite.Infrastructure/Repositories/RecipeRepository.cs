using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DealBite.Infrastructure.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Recipe?> GetByIdWithDetails(Guid Id)
        {
            return await _context.Recipes
                 .Where(r => r.Id == Id)
                 .Include(r => r.Ingredients).ThenInclude(i => i.Product)
                 .Include(r => r.RecipeSteps)
                 .FirstOrDefaultAsync();

        }

        public async Task<RecipeGenerationCache?> GetValidCacheAsync(string mode, Guid? storeId, ProductSegment Segment)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.RecipeGenerationCaches
                .Where(r => r.Mode == mode)
                .Where(s => s.StoreId == storeId)
                .Where(p => p.Segment == Segment)
                .Where(v => v.ValidUntil >= today)
                .Include(rec => rec.Recipes).ThenInclude(i => i.Ingredients).ThenInclude(p=>p.Product)
                .Include(rec=>rec.Recipes).ThenInclude(s=>s.RecipeSteps)
                .FirstOrDefaultAsync();
        }

        public async Task SaveGeneratedRecipesAsync(RecipeGenerationCache cache)
        {
            _context.RecipeGenerationCaches.Add(cache);
            await _context.SaveChangesAsync();
        }
    }
}
