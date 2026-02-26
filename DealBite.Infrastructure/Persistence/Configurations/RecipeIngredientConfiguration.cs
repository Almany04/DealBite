using DealBite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Persistence.Configurations
{
    public class RecipeIngredientConfiguration:IEntityTypeConfiguration<RecipeIngredient>
    {
        public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
        {
            builder.ComplexProperty(sli => sli.SavingsAmount, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount)
                    .HasColumnName("SavingsAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                moneyBuilder.Property(m => m.Currency)
                    .HasColumnName("SavingsAmountCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        }
    }
}
