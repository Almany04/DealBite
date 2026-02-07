using DealBite.Domain.Common;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class AppUser:BaseEntity
    {
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public string? PasswordHash { get; set; }
        public GeoCoordinate? DefaultLocation { get; set; }
        public DateTimeOffset? CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastLoginAt { get; set; }
        public ICollection<ShoppingList> ShoppingLists { get; set; } = [];
    }
}
