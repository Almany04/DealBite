using DealBite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealBite.Infrastructure.Persistence.Configurations
{
    public class ProductPriceConfiguration:IEntityTypeConfiguration<ProductPrice>
    {
        public void Configure(EntityTypeBuilder<ProductPrice> builder)
        {
            builder.ComplexProperty(p => p.Price, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                    .HasColumnName("PriceAmount")
                    .HasPrecision(18, 2);

                moneyBuilder.Property(m => m.Currency)
                    .HasColumnName("PriceCurrency")
                    .HasMaxLength(3);
            });
            builder.ComplexProperty(p => p.OriginalPrice, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                    .HasColumnName("OriginalPriceAmount")
                    .HasPrecision(18, 2);

                moneyBuilder.Property(m => m.Currency)
                    .HasColumnName("OriginalPriceCurrency")
                    .HasMaxLength(3);
            });

            builder.Property(p => p.UnitPriceAmount).HasPrecision(18, 2);
            builder.HasIndex(p => new { p.StoreId, p.IsOnSale });
        }
    }
}
