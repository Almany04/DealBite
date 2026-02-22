using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class ShoppingListOptimizationResultDto
    {
        public Guid ShoppingListId { get; set; }
        public int TotalItemsInList { get; set; }
        public List<StoreOptimizationResultDto> StoreRankings { get; set; } = new();
    }
}
