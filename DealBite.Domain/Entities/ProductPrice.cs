using DealBite.Domain.Common;
using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class ProductPrice: BaseEntity
    {
        public required Money Price { get; set; }
        public Money? OriginalPrice { get; set; }
        public bool IsOnSale { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? UnitPriceAmount { get; set; }
        public DateOnly ValidFrom {  get; set; }
        public DateOnly ValidTo { get; set; }
        public PriceSource Source { get; set; }
        public PromotionType PromotionType { get; set; }
        public string? AppRequired { get; set; }
        public DateTimeOffset LastScrapedAt { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
