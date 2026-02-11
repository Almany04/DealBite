using DealBite.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Persistence.Configurations
{
    public class AppUserConfiguration:IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.Id).ValueGeneratedNever();

            
            builder.Property(x => x.DefaultLocation)
                   .HasColumnType("geography(Point, 4326)");
        }
    }
}
