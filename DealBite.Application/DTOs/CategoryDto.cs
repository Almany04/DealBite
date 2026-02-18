
namespace DealBite.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; init; }
        public string Slug { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public ICollection<CategoryDto> SubCategories { get; set; } = [];
    }
}
