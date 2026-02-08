using DealBite.Domain.Common;
using DealBite.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class Product:BaseEntity
    {
        public required string Name { get; set; }   
        public required string NormalizedName { get; set; }
        public string? Description { get; set; }
        public string? AiGeneratedImageUrl { get; set; }
        public ProductUnit UnitType { get; set; }
        public bool IsIngredient { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
