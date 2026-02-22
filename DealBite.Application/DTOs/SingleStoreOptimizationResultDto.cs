using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class SingleStoreOptimizationResultDto
    {
        public Guid ShoppingListId { get; set; }
        public int TotalItemsInList { get; set; }
        public List<SingleStoreRankingDto> StoreRankings { get; set; } = new();
    }
}
