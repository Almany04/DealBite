using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Results
{
    public class OptimizedItemResult
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal SavedOnItem { get; set; }
        public decimal? ReferencePriceAmount { get; set; }
    }
}
