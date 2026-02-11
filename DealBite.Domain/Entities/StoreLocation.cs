using DealBite.Domain.Common;
using NetTopologySuite.Geometries;


namespace DealBite.Domain.Entities
{
    public class StoreLocation:BaseEntity
    {
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? ZipCode { get; set; }

        public required Point Coordinates { get; set; }

        public Guid StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
