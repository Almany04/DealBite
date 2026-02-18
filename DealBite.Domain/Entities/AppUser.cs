using DealBite.Domain.Common;
using NetTopologySuite.Geometries;

namespace DealBite.Domain.Entities
{
    public class AppUser:BaseEntity
    {
        public required string IdentityUserId { get; set; }
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public Point? DefaultLocation { get; set; }
        public DateTimeOffset? CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastLoginAt { get; set; }
        public ICollection<ShoppingList> ShoppingLists { get; set; } = [];
    }
}
