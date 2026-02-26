
namespace DealBite.Application.DTOs
{
    public class RecipeIngredientDto
    {
        public string IngredientName { get; set; } = string.Empty;
        public decimal SavingsAmount { get; set; }
        public double Amount { get; set; }
        public string UnitType { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public bool IsOnSale { get; set; }
        public Guid RecipeId { get; set; }
        public Guid? ProductId { get; set; }
    }
}
