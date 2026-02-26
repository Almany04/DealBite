using DealBite.Domain.Common;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class Recipe:BaseEntity
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int PrepTimeMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public Money TotalSavings { get; set; } = Money.Zero;
        public int Servings { get; set; }
        public Guid? RecipeGenerationCacheId { get; set; }
        public RecipeGenerationCache? RecipeGenerationCache { get; set; }
        public ICollection<RecipeIngredient> Ingredients { get; set; } = [];
        public ICollection<RecipeStep> RecipeSteps { get; set; } = [];
    }
}
