using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Common.Models
{
    public class GeneratedIngredientData
    {
        public string IngredientName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string UnitType { get; set; } = string.Empty;
        public Guid? ProductId { get; set; }
    }
}
