using DealBite.Domain.Entities;
using DealBite.Domain.Enums;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface IRecipeRepository:IGenericRepository<Recipe>
    {
        Task<Recipe?> GetByIdWithDetails(Guid Id);
        Task<RecipeGenerationCache?> GetValidCacheAsync(string mode, Guid? storeId, ProductSegment Segment);
        Task SaveGeneratedRecipesAsync(RecipeGenerationCache cache);
    }
}
