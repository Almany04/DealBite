using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Results
{
    public class StoreAssignment
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public decimal StoreSubTotal { get; set; }
        public List<OptimizedItemResult> Items { get; set; } = new();
    }
}
