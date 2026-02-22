using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Services;
using DealBite.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.ShoppingLists.Queries.GetShoppingListOptimization
{
    public class GetShoppingListOptimizationQuery : IRequest<ShoppingListOptimizationResultDto>
    {
        public Guid Id { get; set; }
    }
    public class GetShoppingListOptimizationHandler : IRequestHandler<GetShoppingListOptimizationQuery, ShoppingListOptimizationResultDto>
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IPriceHistoryRepository _priceHistoryRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetShoppingListOptimizationHandler(IShoppingListRepository shoppingListRepository, IPriceHistoryRepository priceHistoryRepository, IStoreRepository storeRepository, IProductRepository productRepository, IMapper mapper)
        {
            _shoppingListRepository = shoppingListRepository;
            _priceHistoryRepository = priceHistoryRepository;
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ShoppingListOptimizationResultDto> Handle(GetShoppingListOptimizationQuery request, CancellationToken cancellationToken)
        {
            var shoppinglist = await _shoppingListRepository.GetByIdWithItemsAsync(request.Id);

            if (shoppinglist == null)
                throw new KeyNotFoundException($"Ez a lista nem található: {request.Id}");

            var productIds = shoppinglist.ShoppingListItems.Select(item => item.ProductId).ToList();

            var productsWithPrices = await _productRepository.GetProductsWithPricesAsync(productIds);

            var referencePrices = new Dictionary<Guid, ReferencePrice>();

            foreach (var productId in productIds)
            {
                var priceHistory = await _priceHistoryRepository.GetByProductIdAsync(productId);
                if (priceHistory.Any())
                {
                    var historicPricesList = priceHistory.Select(ph => ph.Price.Amount).ToList();

                    var referencePrice = ReferencePriceCalculator.Calculate(historicPricesList);

                    referencePrices[productId] = referencePrice;
                }
            }

            var storeGroups = productsWithPrices.GroupBy(p => p.StoreId);

            var storeRankings = new List<StoreOptimizationResultDto>();

            foreach (var group in storeGroups)
            {
                var storeOptimization = new StoreOptimizationResultDto
                {
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

                    bool isAvailable = storePriceForProduct != null;
                    decimal unitPrice = isAvailable ? storePriceForProduct!.Price.Amount : 0m;

                    decimal savedOnItem = 0m;

                    if (isAvailable && referencePrices.TryGetValue(listItem.ProductId, out var refPrice))
                        savedOnItem=(refPrice.MedianPrice.Amount - unitPrice) * (decimal)listItem.Quantity;

                    var itemDto = new OptimizedItemDto
                    {
                        ProductId = listItem.ProductId,
                        IsAvailable = isAvailable,
                        Quantity = (decimal)listItem.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = unitPrice * (decimal)listItem.Quantity,
                        SavedOnItem = savedOnItem
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

            var finalResult = new ShoppingListOptimizationResultDto
            {
                ShoppingListId = request.Id,
                TotalItemsInList = shoppinglist.ShoppingListItems.Count,
                StoreRankings = storeRankings.OrderByDescending(s => s.TotalSaved).ToList()
            };

            return finalResult;
        }
    }
}
