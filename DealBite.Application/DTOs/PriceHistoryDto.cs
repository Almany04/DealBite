using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class PriceHistoryDto
    {
        public decimal Price { get; set; }
        public DateTimeOffset RecordedAt { get; set; }
        public string StoreName { get; set; } = string.Empty;
    }
}
