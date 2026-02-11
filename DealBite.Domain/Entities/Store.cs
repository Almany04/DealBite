using DealBite.Domain.Common;
using DealBite.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class Store:BaseEntity
    {
        public required string Name { get; set; }
        public required StoreSlug StoreSlug { get; set; }
        public string? BrandColor { get; set; }
        public string? LogoUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<StoreLocation> Locations { get; set; } = [];

    }
}
