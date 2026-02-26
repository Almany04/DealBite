using DealBite.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Common.Models
{
    public class GeneratedRecipeData
    {
        public string Title { get; set; } = string.Empty;
        public int PrepTimeMinutes { get; set; }
        public string? Description { get; set; }
        public int Servings { get; set; }
        public ICollection<GeneratedStepData> Steps { get; set; } = [];
        public ICollection<GeneratedIngredientData> Ingredients { get; set; } = [];
    }
}
