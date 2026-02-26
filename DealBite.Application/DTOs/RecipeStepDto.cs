

namespace DealBite.Application.DTOs
{
    public class RecipeStepDto
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public Guid RecipeId { get; set; }
    }
}
