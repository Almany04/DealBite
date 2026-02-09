using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class StoreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? BrandColor { get; set; }
        public string? LogoUrl { get; set; }

        public ICollection<StoreLocationDto> Locations { get; set; } = [];

    }
}
