using DealBite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Persistence.Configurations
{
    public class RecipeConfiguration:IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ComplexProperty(sli => sli.TotalSavings, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                    .HasColumnName("TotalSavings")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(m => m.Currency)
                    .HasColumnName("TotalSavingsCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        }
    }
}
