using DealBite.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class Category:BaseEntity
    {
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public string? IconUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = [];
    }
}
