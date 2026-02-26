using DealBite.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class RecipeStep:BaseEntity
    {
        public int StepNumber { get; set; }
        public required string Instruction { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
    }
}
