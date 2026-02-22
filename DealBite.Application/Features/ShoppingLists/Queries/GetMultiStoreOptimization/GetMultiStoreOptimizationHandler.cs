using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Queries.GetMultiStoreOptimization
{
    public class GetMultiStoreOptimizationQuery : IRequest<MultiStoreOptimizationResultDto>
    {
        public Guid Id { get; set; }
        public List<Guid>? StoreIds { get; set; }
    }
    public class GetMultiStoreOptimizationHandler : IRequestHandler<GetMultiStoreOptimizationQuery, MultiStoreOptimizationResultDto>
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IPriceHistoryRepository _priceHistoryRepository;
        private readonly IProductRepository _productRepository;

        public GetMultiStoreOptimizationHandler(IShoppingListRepository shoppingListRepository, IPriceHistoryRepository priceHistoryRepository, IProductRepository productRepository)
        {
            _shoppingListRepository = shoppingListRepository;
            _priceHistoryRepository = priceHistoryRepository;
            _productRepository = productRepository;
        }

        public async Task<MultiStoreOptimizationResultDto> Handle(GetMultiStoreOptimizationQuery request, CancellationToken cancellationToken)
        {
            var shoppingList = await _shoppingListRepository.GetByIdWithItemsAsync(request.Id);
            if (shoppingList == null)
                throw new KeyNotFoundException($"Ez a lista nem található: {request.Id}");

            var productIds = shoppingList.ShoppingListItems.Select(item => item.ProductId).ToList();

            var allPrices = await _productRepository.GetProductsWithPricesAsync(productIds);

            var referencePrices = new Dictionary<Guid, decimal>();

            foreach (var productId in productIds)
            {
                var priceHistory = await _priceHistoryRepository.GetByProductIdAsync(productId);
                if (priceHistory.Any())
                {
                    var historicPricesList = priceHistory.Select(ph => ph.Price.Amount).ToList();

                    var referencePrice = ReferencePriceCalculator.Calculate(historicPricesList);

                    referencePrices[productId] = referencePrice.MedianPrice.Amount;
                }
            }

            var comboResults = MultiStoreOptimizer.Optimize(
                shoppingList.ShoppingListItems.ToList(), allPrices, referencePrices, request.StoreIds);

            var storeComboDtos = comboResults.Select(combo => new StoreComboResultDto
            {
                StoreComboCount = combo.StoreComboCount,
                AvailableItemsCount = combo.AvailableItemsCount,
                TotalEstimatedPrice = combo.TotalEstimatedPrice,
                TotalSaved = combo.TotalSaved,
                ReferencePriceAmount = combo.ReferencePriceAmount,
                UnavailableItems = combo.UnavailableItems.Select(u => new OptimizedItemDto
                {
                    ProductId = u.ProductId,
                    ProductName = u.ProductName,
                    Quantity = u.Quantity,
                    IsAvailable = false
                }).ToList(),
                StoreAssignments=combo.StoreAssignments.Select(a=>new StoreAssignmentDto
                {
                    StoreId=a.StoreId,
                    StoreName=a.StoreName,
                    LogoUrl=a.LogoUrl,
                    StoreSubTotal=a.StoreSubTotal,
                    Items=a.Items.Select(i=>new OptimizedItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        IsAvailable = true,
                        UnitPrice= i.UnitPrice,
                        TotalPrice=i.TotalPrice,
                        SavedOnItem=i.SavedOnItem,
                        ReferencePriceAmount=i.ReferencePriceAmount
                    }).ToList()
                }).ToList()
            }).ToList();

            return new MultiStoreOptimizationResultDto
            {
                ShoppingListId = shoppingList.Id,
                TotalItemsInList = shoppingList.ShoppingListItems.Count,
                StoreCombos = storeComboDtos
            };
        }
    }
}
