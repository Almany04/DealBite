using DealBite.Domain.Common;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class PriceHistory:BaseEntity
    {
        public required Money Price { get; set; }
        public DateTimeOffset RecordedAt { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
