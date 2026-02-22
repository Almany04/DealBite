using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class MultiStoreOptimizationResultDto
    {
        public Guid ShoppingListId { get; set; }
        public int TotalItemsInList { get; set; }
        public List<StoreComboResultDto> StoreCombos { get; set; } = new();
    }
}
