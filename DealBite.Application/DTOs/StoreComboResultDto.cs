using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class StoreComboResultDto
    {
        public decimal? ReferencePriceAmount { get; set; }
        public int StoreComboCount { get; set; }
        public int AvailableItemsCount { get; set; }
        public decimal TotalEstimatedPrice { get; set; }
        public decimal TotalSaved { get; set; }
        public List<OptimizedItemDto> UnavailableItems { get; set; } = new();
        public List<StoreAssignmentDto> StoreAssignments { get; set; } = new();
    }
}
