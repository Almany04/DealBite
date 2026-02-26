using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Application.Interfaces.Services;
using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;

namespace DealBite.Application.Features.Recipes.Queries
{
    public class GetRecommendedRecipesQuery : IRequest<List<RecommendedRecipeDto>>
    {
        public string Mode { get; set; } = "single";
        public Guid? StoreId { get; set; }
        public ProductSegment Segment { get; set; }
    }
    public class GetRecommendedRecipesHandler : IRequestHandler<GetRecommendedRecipesQuery, List<RecommendedRecipeDto>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPriceHistoryRepository _priceHistoryRepository;
        private readonly IRecipeGeneratorService _recipeGeneratorService;
        private readonly IMapper _mapper;

        public GetRecommendedRecipesHandler(IRecipeRepository recipeRepository, IProductRepository productRepository, IPriceHistoryRepository priceHistoryRepository, IRecipeGeneratorService recipeGeneratorService, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _productRepository = productRepository;
            _priceHistoryRepository = priceHistoryRepository;
            _recipeGeneratorService = recipeGeneratorService;
            _mapper = mapper;
        }

        public async Task<List<RecommendedRecipeDto>> Handle(GetRecommendedRecipesQuery request, CancellationToken cancellationToken)
        {
            var recipes = await _recipeRepository.GetValidCacheAsync(request.Mode, request.StoreId, request.Segment);
            if (recipes != null)
            {
                return _mapper.Map<List<RecommendedRecipeDto>>(recipes.Recipes.ToList());
            }
            var onSaleProducts = await _productRepository.GetOnSaleProductPricesAsync(request.StoreId, request.Segment);

            if (!onSaleProducts.Any())
            {
                return new List<RecommendedRecipeDto>();
            }
            var generatedRecipes = await _recipeGeneratorService.GenerateRecipesAsync(onSaleProducts, request.Segment);

            var recipeEntities = new List<Recipe>();
            foreach (var generated in generatedRecipes)
            {
                var recipe = new Recipe
                {
                    Title = generated.Title,
                    Description = generated.Description,
                    PrepTimeMinutes = generated.PrepTimeMinutes,
                    Servings = generated.Servings,
                    TotalSavings = Money.Zero
                };
                foreach (var ing in generated.Ingredients)
                {
                    var ingredient = new RecipeIngredient
                    {
                        IngredientName=ing.IngredientName,
                        Amount=ing.Amount,
                        UnitType=Enum.TryParse<ProductUnit>(ing.UnitType, out var unit)?unit:ProductUnit.Db,
                        ProductId=ing.ProductId,
                        SavingsAmount=Money.Zero,
                        RecipeId=recipe.Id
                    };
                    if (ing.ProductId.HasValue)
                    {
                        var productPrice = onSaleProducts.FirstOrDefault(p => p.ProductId == ing.ProductId.Value);
                        if (productPrice != null)
                        {
                            ingredient.StoreName = productPrice.Store?.Name;
                            var priceHistory = await _priceHistoryRepository
                                .GetByProductIdWithTimeLimitAsync(ing.ProductId.Value, DateTimeOffset.UtcNow.AddDays(-56));
                            if (priceHistory.Any())
                            {
                                var referencePrice = ReferencePriceCalculator.Calculate(priceHistory.Select(ph => ph.Price.Amount).ToList());

                                var savings = (referencePrice.MedianPrice.Amount - productPrice.Price.Amount) * (decimal)ing.Amount;

                                if (savings > 0)
                                {
                                    ingredient.SavingsAmount = new Money(savings);
                                }
                            }
                        }
                    }
                    recipe.Ingredients.Add(ingredient);
                }
                foreach (var step in generated.Steps)
                {
                    recipe.RecipeSteps.Add(new RecipeStep
                    {
                        StepNumber=step.StepNumber,
                        Instruction=step.Instruction,
                        RecipeId=recipe.Id
                    });
                }

                recipe.TotalSavings = new Money(recipe.Ingredients.Sum(i => i.SavingsAmount.Amount));

                recipeEntities.Add(recipe);
            }
            var validUntil = onSaleProducts.Min(p => p.ValidTo);

            var cache = new RecipeGenerationCache
            {
                Mode = request.Mode,
                StoreId = request.StoreId,
                Segment = request.Segment,
                GeneratedAt = DateTimeOffset.UtcNow,
                ValidUntil = validUntil,
                Recipes = recipeEntities
            };

            recipeEntities = recipeEntities.OrderByDescending(r => r.TotalSavings.Amount).ToList();

            await _recipeRepository.SaveGeneratedRecipesAsync(cache);

            return _mapper.Map<List<RecommendedRecipeDto>>(recipeEntities);
        }
    }
}
