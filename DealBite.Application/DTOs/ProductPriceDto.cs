using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class ProductPriceDto
    {
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public bool IsOnSale { get; set; }
        public int? DiscountPercent { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly ValidTo { get; set; }
        public string PriceSource { get; set; } =string.Empty;
    }
}
