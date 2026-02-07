using DealBite.Domain.Common;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class StoreLocation:BaseEntity
    {
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? ZipCode { get; set; }

        public GeoCoordinate Coordinate { get; set; }

        public Guid StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
