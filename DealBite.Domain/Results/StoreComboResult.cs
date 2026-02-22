using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Results
{
    public class StoreComboResult
    {
        public decimal? ReferencePriceAmount { get; set; }
        public int StoreComboCount { get; set; }
        public int AvailableItemsCount { get; set; }
        public decimal TotalEstimatedPrice { get; set; }
        public decimal TotalSaved { get; set; }
        public List<OptimizedItemResult> UnavailableItems { get; set; } = new();
        public List<StoreAssignment> StoreAssignments { get; set; } = new();
    }
}
