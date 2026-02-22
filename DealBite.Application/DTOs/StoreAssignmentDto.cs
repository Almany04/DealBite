using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class StoreAssignmentDto
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public decimal StoreSubTotal { get; set; }
        public List<OptimizedItemDto> Items { get; set; } = new();
    }
}
