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
            builder.Ignore(x => x.Coordinates);

            builder.Property<Point>("Location")
                .HasColumnName("Coordinates")
                .HasColumnType("geography(Point, 4326)")
                .IsRequired();
        }
    }
}
