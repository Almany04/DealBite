using DealBite.Domain.Entities;
using DealBite.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Persistence
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationDbUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreLocation> StoreLocations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }


        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }


        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("postgis");
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        
    }
}
