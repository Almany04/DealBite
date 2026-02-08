using DealBite.Domain.Entities;
using DealBite.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace DealBite.Infrastructure.Persistence.Configurations
{
    public class StoreLocationConfiguration: IEntityTypeConfiguration<StoreLocation>
    {
        public void Configure(EntityTypeBuilder<StoreLocation> builder)
        {
            builder.Property(x => x.Coordinates)
            .HasConversion(
                // C# -> DB: GeoCoordinate-ből Point lesz
                c => new Point(c.Longitude, c.Latitude) { SRID = 4326 },
                // DB -> C#: Point-ból GeoCoordinate lesz
                p => new GeoCoordinate(p.Y, p.X)
            )
            .HasColumnType("geography(Point, 4326)");
        }
    }
}
