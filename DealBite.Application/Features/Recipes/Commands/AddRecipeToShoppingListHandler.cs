using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Recipes.Commands
{
    public class AddRecipeToShoppingListCommand : IRequest<Unit>
    {
        public Guid RecipeId { get; set; }
        public Guid ShoppingListId { get; set; }
        public List<Guid>? SelectedIngredientsIds { get; set; }
    }
    public class AddRecipeToShoppingListHandler : IRequestHandler<AddRecipeToShoppingListCommand, Unit>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IShoppingListItemRepository _shoppingListItemRepository;
        private readonly IProductRepository _productRepository;

        public AddRecipeToShoppingListHandler(
            IRecipeRepository recipeRepository,
            IShoppingListRepository shoppingListRepository,
            IShoppingListItemRepository shoppingListItemRepository,
            IProductRepository productRepository)
        {
            _recipeRepository = recipeRepository;
            _shoppingListRepository = shoppingListRepository;
            _shoppingListItemRepository = shoppingListItemRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(AddRecipeToShoppingListCommand request, CancellationToken cancellationToken)
        {

            var recipe = await _recipeRepository.GetByIdWithDetails(request.RecipeId);
            if (recipe == null)
                throw new KeyNotFoundException($"Recept nem található: {request.RecipeId}");

            var shoppingList = await _shoppingListRepository.GetByIdWithItemsAsync(request.ShoppingListId);
            if (shoppingList == null)
                throw new KeyNotFoundException($"Bevásárlólista nem található: {request.ShoppingListId}");

            var ingredientsToAdd = request.SelectedIngredientsIds == null
                                   || !request.SelectedIngredientsIds.Any()
                ? recipe.Ingredients.ToList()
                : recipe.Ingredients
                    .Where(i => request.SelectedIngredientsIds.Contains(i.Id))
                    .ToList();


            foreach (var ingredient in ingredientsToAdd)
            {
                if (ingredient.ProductId.HasValue)
                {

                    var existing = shoppingList.ShoppingListItems
                        .FirstOrDefault(i => i.ProductId == ingredient.ProductId);

                    if (existing != null)
                    {
                        existing.Quantity += ingredient.Amount;

                        var cheapestPrice = await _productRepository
                            .GetEstimatedPriceMinimumWithDetailsAsync(ingredient.ProductId.Value);

                        existing.EstimatedPrice = (cheapestPrice?.Price ?? Money.Zero) * (decimal)existing.Quantity;

                        await _shoppingListItemRepository.UpdateAsync(existing);
                    }
                    else
                    {
                        var cheapestPrice = await _productRepository
                            .GetEstimatedPriceMinimumWithDetailsAsync(ingredient.ProductId.Value);

                        var newItem = new ShoppingListItem
                        {
                            ProductName = ingredient.IngredientName,
                            Quantity = ingredient.Amount,
                            IsChecked = false,
                            EstimatedPrice = (cheapestPrice?.Price ?? Money.Zero) * (decimal)ingredient.Amount,
                            ProductId = ingredient.ProductId,
                            StoreId = null,
                            ShoppingListId = request.ShoppingListId
                        };

                        await _shoppingListItemRepository.AddAsync(newItem);
                    }
                }
                else
                {
                    var newItem = new ShoppingListItem
                    {
                        ProductName = ingredient.IngredientName,
                        Quantity = ingredient.Amount,
                        IsChecked = false,
                        EstimatedPrice = Money.Zero,
                        ProductId = null,
                        StoreId = null,
                        ShoppingListId = request.ShoppingListId
                    };

                    await _shoppingListItemRepository.AddAsync(newItem);
                }
            }

            var updatedList = await _shoppingListRepository.GetByIdWithItemsAsync(request.ShoppingListId);
            var totals = ShoppingListCalculator.ShoppingCalculator(
                updatedList!.ShoppingListItems.ToList().AsReadOnly());

            var listToUpdate = await _shoppingListRepository.GetByIdAsync(request.ShoppingListId);
            listToUpdate!.TotalEstimatedPrice = totals.TotalEstimatedPrice;
            listToUpdate.TotalSaved = totals.TotalSaved;

            await _shoppingListRepository.UpdateAsync(listToUpdate);

            return Unit.Value;
        }
    }
}
