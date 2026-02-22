using DealBite.Domain.Entities;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Services
{
    public static class ShoppingListCalculator
    {
        
        public static (Money TotalEstimatedPrice, Money TotalSaved) ShoppingCalculator(IReadOnlyList<ShoppingListItem> shoppingLists)
        {
            var estimated = Money.Zero;
            foreach (var item in shoppingLists)
            {
                estimated += item.EstimatedPrice;
            }

            return (estimated, Money.Zero);
        }
    }
}
