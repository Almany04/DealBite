using DealBite.Application.Common.Models;
using DealBite.Domain.Entities;
using DealBite.Domain.Enums;

namespace DealBite.Application.Interfaces.Services
{
    public interface IRecipeGeneratorService
    {
        Task<List<GeneratedRecipeData>> GenerateRecipesAsync(List<ProductPrice> onSaleProducts, ProductSegment segment, int count = 10);
    }
}
