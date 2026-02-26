using DealBite.Domain.Common;
using DealBite.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class RecipeGenerationCache:BaseEntity
    {
        public required string Mode { get; set; }
        public Guid? StoreId { get; set; }
        public ProductSegment Segment { get; set; }
        public DateTimeOffset GeneratedAt { get; set; }
        public DateOnly ValidUntil { get; set; }
        public ICollection<Recipe> Recipes { get; set; } = [];
    }
}
