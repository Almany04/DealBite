using DealBite.Domain.Entities;
using DealBite.Domain.Results;

namespace DealBite.Domain.Services
{
    public static class MultiStoreOptimizer
    {
        private static void Generate(List<Guid> storeIds, int k, int startIndex, List<Guid> currentCombo, List<List<Guid>> results)
        {
            if (currentCombo.Count == k)
            {
                results.Add(new List<Guid>(currentCombo));
                return;
            }
            for (int i = startIndex; i < storeIds.Count; i++)
            {
                currentCombo.Add(storeIds[i]);
                Generate(storeIds, k, i + 1, currentCombo, results);
                currentCombo.RemoveAt(currentCombo.Count - 1);
            }
        }

        private static List<List<Guid>> GenerateCombinations(List<Guid> storeIds, int maxK)
        {
            var results = new List<List<Guid>>();
            for (int k = 1; k <= maxK; k++)
            {
                Generate(storeIds, k, 0, new List<Guid>(), results);
            }
            return results;
        }

        public static List<StoreComboResult> Optimize(
            List<ShoppingListItem> items,
            List<ProductPrice> prices,
            Dictionary<Guid, decimal> referencePrices,
            List<Guid>? storeFilter = null,
            int maxStoresPerCombo = 3,
            int topN = 10)
        {
            if (storeFilter != null)
            {
                prices = prices.Where(p => storeFilter.Contains(p.StoreId)).ToList();
            }

            var storeIds = prices.Select(p => p.StoreId).Distinct().ToList();
            var combinations = GenerateCombinations(storeIds, maxStoresPerCombo);
            var comboResults = new List<StoreComboResult>();

            foreach (var combo in combinations)
            {
                var storeAssignments = new Dictionary<Guid, StoreAssignment>();
                var unavailableItems = new List<OptimizedItemResult>();

                foreach (var listItem in items)
                {
                    // listItem.ProductId itt már Guid? de items-t szűrtük HasValue-ra
                    var productId = listItem.ProductId!.Value;

                    var cheapest = prices
                        .Where(p => combo.Contains(p.StoreId) && p.ProductId == productId)
                        .OrderBy(p => p.Price.Amount)
                        .FirstOrDefault();

                    if (cheapest == null)
                    {
                        unavailableItems.Add(new OptimizedItemResult
                        {
                            IsAvailable = false,
                            ProductId = productId,
                            ProductName = listItem.ProductName,
                            Quantity = (decimal)listItem.Quantity
                        });
                    }
                    else
                    {
                        var cheapestHasValue = new OptimizedItemResult
                        {
                            ProductId = productId,
                            ProductName = listItem.ProductName,
                            Quantity = (decimal)listItem.Quantity,
                            IsAvailable = true,
                            UnitPrice = cheapest.Price.Amount,
                            TotalPrice = cheapest.Price.Amount * (decimal)listItem.Quantity
                        };

                        if (referencePrices.TryGetValue(productId, out var refPrice))
                        {
                            cheapestHasValue.ReferencePriceAmount = refPrice;
                            cheapestHasValue.SavedOnItem = (refPrice - cheapestHasValue.UnitPrice) * (decimal)listItem.Quantity;
                        }

                        if (!storeAssignments.ContainsKey(cheapest.StoreId))
                        {
                            storeAssignments[cheapest.StoreId] = new StoreAssignment
                            {
                                StoreId = cheapest.StoreId,
                                StoreName = cheapest.Store!.Name,
                                LogoUrl = cheapest.Store!.LogoUrl
                            };
                        }

                        storeAssignments[cheapest.StoreId].Items.Add(cheapestHasValue);
                    }
                }

                foreach (var assignment in storeAssignments.Values)
                {
                    assignment.StoreSubTotal = assignment.Items.Sum(i => i.TotalPrice);
                }

                comboResults.Add(new StoreComboResult
                {
                    StoreComboCount = combo.Count,
                    AvailableItemsCount = items.Count - unavailableItems.Count,
                    TotalEstimatedPrice = storeAssignments.Values.Sum(a => a.StoreSubTotal),
                    TotalSaved = storeAssignments.Values.SelectMany(a => a.Items).Sum(i => i.SavedOnItem),
                    UnavailableItems = unavailableItems,
                    StoreAssignments = storeAssignments.Values.ToList(),
                    ReferencePriceAmount = storeAssignments.Values
                        .SelectMany(a => a.Items)
                        .Sum(i => (i.ReferencePriceAmount ?? i.UnitPrice) * i.Quantity)
                });
            }

            return comboResults
                .OrderByDescending(c => c.AvailableItemsCount)
                .ThenBy(c => c.TotalEstimatedPrice)
                .Take(topN)
                .ToList();
        }
    }
}