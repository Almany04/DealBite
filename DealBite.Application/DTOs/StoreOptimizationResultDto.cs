using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class StoreOptimizationResultDto
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int AvailableItemsCount { get; set; }
        public decimal TotalEstimatedPrice { get; set; }
        public decimal TotalSaved { get; set; }
        public List<OptimizedItemDto> Items { get; set; } = new();
    }
}
