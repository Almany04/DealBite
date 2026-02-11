using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects; // A Money miatt ez marad!
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries; // <--- EZT ADTAM HOZZÁ a Point miatt
using System;
using System.Threading.Tasks;

namespace DealBite.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Scope létrehozása, hogy biztosan megkapjuk a Contextet
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS INDUL ---");

            // Migrációk automatikus futtatása induláskor
            await context.Database.MigrateAsync();

            // Ha már vannak boltok, nem futtatjuk újra a seed-et (opcionális biztonsági ellenőrzés)
            // De mivel a te kódodban törlés van, ezt most kihagyom, hogy mindig újrahúzza a tesztadatokat.

            // 1. TAKARÍTÁS (Sorrend fontos a foreign key-ek miatt!)
            Console.WriteLine(">>> RÉGI ADATOK TÖRLÉSE... <<<");

            // Törlés hatékonyabban:
            if (context.ProductPrices.Any()) context.ProductPrices.RemoveRange(context.ProductPrices);
            if (context.ShoppingListItems.Any()) context.ShoppingListItems.RemoveRange(context.ShoppingListItems);
            if (context.ShoppingLists.Any()) context.ShoppingLists.RemoveRange(context.ShoppingLists);
            if (context.RecipeIngredients.Any()) context.RecipeIngredients.RemoveRange(context.RecipeIngredients);
            if (context.Recipes.Any()) context.Recipes.RemoveRange(context.Recipes);
            if (context.PriceHistories.Any()) context.PriceHistories.RemoveRange(context.PriceHistories);
            if (context.Products.Any()) context.Products.RemoveRange(context.Products);
            if (context.StoreLocations.Any()) context.StoreLocations.RemoveRange(context.StoreLocations);
            if (context.Stores.Any()) context.Stores.RemoveRange(context.Stores);
            if (context.Categories.Any()) context.Categories.RemoveRange(context.Categories);

            await context.SaveChangesAsync();

            // 2. KATEGÓRIÁK
            Console.WriteLine(">>> KATEGÓRIÁK LÉTREHOZÁSA... <<<");
            var catDairy = new Category { Name = "Tejtermékek", Slug = "tejtermekek" };
            var catBakery = new Category { Name = "Pékáru", Slug = "pekaru" };
            var catMeat = new Category { Name = "Húsok", Slug = "husok" };

            await context.Categories.AddRangeAsync(catDairy, catBakery, catMeat);
            await context.SaveChangesAsync();

            // 3. BOLTOK ÉS HELYSZÍNEK
            Console.WriteLine(">>> BOLTOK LÉTREHOZÁSA... <<<");

            var lidl = new Store
            {
                Name = "Lidl",
                StoreSlug = StoreSlug.Lidl, // Figyelj a kisbetűs/nagybetűs property névre a Store entitásban!
                BrandColor = "#0050AA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/91/Lidl-Logo.svg",
                IsActive = true,
                WebsiteUrl = "https://www.lidl.hu"
            };

            // JAVÍTVA: Point használata GeoCoordinate helyett
            // FONTOS: (Longitude, Latitude) sorrend! (Keleti hosszúság, Északi szélesség)
            lidl.Locations.Add(new StoreLocation
            {
                Address = "Király u. 112.",
                City = "Budapest",
                ZipCode = "1068",
                // Budapest: X=19.0683 (Lon), Y=47.5069 (Lat)
                Coordinates = new Point(19.0683, 47.5069) { SRID = 4326 }
            });

            lidl.Locations.Add(new StoreLocation
            {
                Address = "Csalogány u. 43.",
                City = "Budapest",
                ZipCode = "1027",
                Coordinates = new Point(19.0347, 47.5098) { SRID = 4326 }
            });

            var spar = new Store
            {
                Name = "Spar",
                StoreSlug = StoreSlug.Spar,
                BrandColor = "#007A33",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/22/Spar-Logo.svg/1200px-Spar-Logo.svg.png",
                IsActive = true,
                WebsiteUrl = "https://www.spar.hu"
            };

            spar.Locations.Add(new StoreLocation
            {
                Address = "Batthyány tér 5-6.",
                City = "Budapest",
                ZipCode = "1011",
                Coordinates = new Point(19.0384, 47.5055) { SRID = 4326 }
            });

            await context.Stores.AddRangeAsync(lidl, spar);
            await context.SaveChangesAsync();

            // 4. TERMÉKEK
            Console.WriteLine(">>> TERMÉKEK LÉTREHOZÁSA... <<<");
            var milk = new Product
            {
                Name = "Pilos Tej 2,8%",
                NormalizedName = "pilos tej 2,8%",
                // Quantity = 1, // Ha nincs ilyen property a Product entitásban, vedd ki!
                UnitType = ProductUnit.L,
                AiGeneratedImageUrl = "https://example.com/milk.jpg",
                IsIngredient = true,
                CategoryId = catDairy.Id,
                Category = catDairy
            };

            await context.Products.AddAsync(milk);
            await context.SaveChangesAsync();

            // 5. ÁRAK
            Console.WriteLine(">>> ÁRAK HOZZÁRENDELÉSE... <<<");

            // Megjegyzés: A ProductPrice entitásodban a Price ComplexType (Money).
            // Az "Amount" és "Currency" property-ket az EF Core mappolja be a Money struct-ból.

            var milkPriceLidl = new ProductPrice
            {
                ProductId = milk.Id,
                Product = milk,
                StoreId = lidl.Id,
                Store = lidl,
                Price = new Money(299, "HUF"),
                Source = PriceSource.Manual,
                LastScrapedAt = DateTimeOffset.UtcNow,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                IsOnSale = false,
                UnitPriceAmount = 299
            };

            var milkPriceSpar = new ProductPrice
            {
                ProductId = milk.Id,
                Product = milk,
                StoreId = spar.Id,
                Store = spar,
                Price = new Money(349, "HUF"),
                Source = PriceSource.Manual,
                LastScrapedAt = DateTimeOffset.UtcNow,
                ValidFrom = DateOnly.FromDateTime(DateTime.Now),
                ValidTo = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                IsOnSale = false,
                UnitPriceAmount = 349
            };

            await context.ProductPrices.AddRangeAsync(milkPriceLidl, milkPriceSpar);
            await context.SaveChangesAsync();

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS SIKERES! ---");
        }
    }
}