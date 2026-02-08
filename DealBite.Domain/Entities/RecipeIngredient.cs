using DealBite.Domain.Common;
using DealBite.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class RecipeIngredient:BaseEntity
    {
        public double Amount { get; set; }
        public ProductUnit UnitType { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
