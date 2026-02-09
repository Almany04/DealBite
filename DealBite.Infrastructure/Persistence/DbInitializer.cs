using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
           
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var storeRepository = serviceProvider.GetRequiredService<IStoreRepository>();

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS INDUL ---");
            await context.Database.MigrateAsync();


            // --- ITT A VÁLTOZÁS: ELŐSZÖR MINDENT TÖRLÜNK ---
            Console.WriteLine(">>> RÉGI ADATOK TÖRLÉSE... <<<");
            context.StoreLocations.RemoveRange(context.StoreLocations);
            context.ProductPrices.RemoveRange(context.ProductPrices); // Ha már van ilyen
            context.Products.RemoveRange(context.Products);
            context.Stores.RemoveRange(context.Stores);
            context.Categories.RemoveRange(context.Categories);
            await context.SaveChangesAsync();
            // -----------------------------------------------

            Console.WriteLine(">>> ÚJ ADATOK BETÖLTÉSE... <<<");
            Console.WriteLine("--- ADATOK FELTÖLTÉSE FOLYAMATBAN... ---");
            var catDairy = new Category { Name = "Tejtermékek", Slug = "tejtermekek" };
            var catBakery = new Category { Name = "Pékáru", Slug = "pekaru" };
            var catMeat = new Category { Name = "Húsok", Slug = "husok" };

            await context.Categories.AddRangeAsync(catDairy, catBakery, catMeat);
            await context.SaveChangesAsync();

            

            
            var lidl = new Store
            {
                Name = "Lidl",
                StoreSlug = StoreSlug.Lidl,
                BrandColor = "#0050AA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/91/Lidl-Logo.svg"
            };

            
            lidl.Locations.Add(new StoreLocation
            {
                Address = "Király u. 112.",
                City = "Budapest",
                ZipCode = "1068",
                Coordinates = new GeoCoordinate(47.5069, 19.0683)
            });

            lidl.Locations.Add(new StoreLocation
            {
                Address = "Csalogány u. 43.",
                City = "Budapest",
                ZipCode = "1027",
                Coordinates = new GeoCoordinate(47.5098, 19.0347)
            });

            
            var spar = new Store
            {
                Name = "Spar",
                StoreSlug = StoreSlug.Spar,
                BrandColor = "#007A33",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/22/Spar-Logo.svg/1200px-Spar-Logo.svg.png"
            };

            spar.Locations.Add(new StoreLocation
            {
                Address = "Batthyány tér 5-6.",
                City = "Budapest",
                ZipCode = "1011",
                Coordinates = new GeoCoordinate(47.5055, 19.0384)
            });

            
            await storeRepository.AddAsync(lidl);
            await storeRepository.AddAsync(spar);

            
            var milk = new Product
            {
                Name = "Pilos Tej 2,8%",
                NormalizedName = "pilos tej 2,8%",
                Quantity = 1,         
                UnitType = ProductUnit.L,
                AiGeneratedImageUrl = "https://example.com/milk.jpg",
                IsIngredient = true,    
                CategoryId = catDairy.Id
            };

            await context.Products.AddAsync(milk);
            await context.SaveChangesAsync();
            Console.WriteLine("--- ADATOK SIKERESEN MENTVE ---");
        }
    }
}
