
namespace DealBite.Application.DTOs
{
    public class ShoppingListItemDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public bool IsChecked { get; set; }
        public decimal EstimatedPrice { get; set; }
        public Guid ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public Guid StoreId { get; set; }
        public StoreDto? Store { get; set; }
    }
}
