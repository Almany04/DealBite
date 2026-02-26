
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class RecommendedRecipeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PrepTimeMinutes { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal TotalSavings { get; set; }
        public int Servings { get; set; }
        public ICollection<RecipeStepDto> RecipeSteps { get; set; } = [];
        public ICollection<RecipeIngredientDto> Ingredients { get; set; } = [];
    }
}
