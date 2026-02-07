using DealBite.Domain.Common;
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
        public ICollection<RecipeIngredients> Ingredients { get; set; }
    }
}
