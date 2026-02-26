using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Queries.GetSingleStoreOptimization
{
    public class GetSingleStoreOptimizationQuery : IRequest<SingleStoreOptimizationResultDto>
    {
        public Guid Id { get; set; }
    }
    public class GetSingleStoreOptimizationHandler : IRequestHandler<GetSingleStoreOptimizationQuery, SingleStoreOptimizationResultDto>
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IPriceHistoryRepository _priceHistoryRepository;
        private readonly IProductRepository _productRepository;

        public GetSingleStoreOptimizationHandler(IShoppingListRepository shoppingListRepository, IPriceHistoryRepository priceHistoryRepository, IProductRepository productRepository)
        {
            _shoppingListRepository = shoppingListRepository;
            _priceHistoryRepository = priceHistoryRepository;
            _productRepository = productRepository;
        }

        public async Task<SingleStoreOptimizationResultDto> Handle(GetSingleStoreOptimizationQuery request, CancellationToken cancellationToken)
        {
            var shoppinglist = await _shoppingListRepository.GetByIdWithItemsAsync(request.Id);

            if (shoppinglist == null)
                throw new KeyNotFoundException($"Ez a lista nem található: {request.Id}");

            var productIds = shoppinglist.ShoppingListItems.Select(item => item.ProductId).ToList();

            var productsWithPrices = await _productRepository.GetProductsWithPricesAsync(productIds);

            var referencePrices = new Dictionary<Guid, ReferencePrice>();

            foreach (var productId in productIds)
            {
                var priceHistory = await _priceHistoryRepository.GetByProductIdWithTimeLimitAsync(productId, DateTimeOffset.UtcNow.AddDays(-56));
                if (priceHistory.Any())
                {
                    var historicPricesList = priceHistory.Select(ph => ph.Price.Amount).ToList();

                    var referencePrice = ReferencePriceCalculator.Calculate(historicPricesList);

                    referencePrices[productId] = referencePrice;
                }
            }

            var storeGroups = productsWithPrices.GroupBy(p => p.StoreId);

            var storeRankings = new List<SingleStoreRankingDto>();

            foreach (var group in storeGroups)
            {
                var storeOptimization = new SingleStoreRankingDto
                {
                    LogoUrl= group.First().Store!.LogoUrl,
                    StoreId = group.Key,
                    StoreName = group.First().Store!.Name,
                    AvailableItemsCount = 0,
                    TotalEstimatedPrice = 0,
                    TotalSaved = 0,
                    Items = new List<OptimizedItemDto>()
                };

                foreach (var listItem in shoppinglist.ShoppingListItems)
                {
                    var storePriceForProduct = group.FirstOrDefault(p => p.ProductId == listItem.ProductId);

                    decimal? devPercent = null;
                    string? priceEval = null;

                    bool isAvailable = storePriceForProduct != null;
                    decimal unitPrice = isAvailable ? storePriceForProduct!.Price.Amount : 0m;

                    decimal savedOnItem = 0m;
                    decimal? refAmount = referencePrices.TryGetValue(listItem.ProductId, out var refPrice)
                                        ? refPrice.MedianPrice.Amount
                                        : null;
                    if (isAvailable && refAmount.HasValue)
                    {
                        var evaluation = PriceEvaluator.PriceCalculator(unitPrice, refAmount);
                        devPercent = evaluation.DeviationPercent;
                        priceEval = evaluation.priceEvaluation.ToString();
                    }

                    var itemDto = new OptimizedItemDto
                    {
                        ProductName=listItem.ProductName,
                        ProductId = listItem.ProductId,
                        IsAvailable = isAvailable,
                        Quantity = (decimal)listItem.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = unitPrice * (decimal)listItem.Quantity,
                        SavedOnItem = savedOnItem,
                        ReferencePriceAmount = refAmount,
                        DeviationPercent = devPercent,
                        PriceEvaluation = priceEval
                    };

                    storeOptimization.Items.Add(itemDto);

                    storeOptimization.TotalEstimatedPrice += itemDto.TotalPrice;
                    storeOptimization.TotalSaved += itemDto.SavedOnItem;

                    if (isAvailable)
                    {
                        storeOptimization.AvailableItemsCount++;
                    }
                }
                storeRankings.Add(storeOptimization);
            }

            var finalResult = new SingleStoreOptimizationResultDto
            {
                ShoppingListId = request.Id,
                TotalItemsInList = shoppinglist.ShoppingListItems.Count,
                StoreRankings = storeRankings.OrderByDescending(s => s.AvailableItemsCount).ThenByDescending(s=>s.TotalEstimatedPrice).ToList()
            };

            return finalResult;
        }
    }
}
