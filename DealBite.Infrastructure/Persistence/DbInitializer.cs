using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace DealBite.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS INDUL ---");

            await context.Database.MigrateAsync();

            // 1. TAKARÍTÁS
            Console.WriteLine(">>> RÉGI ADATOK TÖRLÉSE... <<<");

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
            var catDairyYogurt = new Category { Name = "Joghurtok", Slug = "joghurtok", ParentCategory = catDairy };
            var catDairyCheese = new Category { Name = "Sajtok", Slug = "sajtok", ParentCategory = catDairy };

            await context.Categories.AddRangeAsync(catDairy, catBakery, catMeat, catDairyYogurt, catDairyCheese);
            await context.SaveChangesAsync();

            // 3. BOLTOK ÉS HELYSZÍNEK
            Console.WriteLine(">>> BOLTOK LÉTREHOZÁSA... <<<");

            var lidl = new Store
            {
                Name = "Lidl",
                StoreSlug = StoreSlug.Lidl,
                BrandColor = "#0050AA",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/91/Lidl-Logo.svg",
                IsActive = true,
                WebsiteUrl = "https://www.lidl.hu"
            };
            lidl.Locations.Add(new StoreLocation
            {
                Address = "Király u. 112.",
                City = "Budapest",
                ZipCode = "1068",
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

            var aldi = new Store
            {
                Name = "Aldi",
                StoreSlug = StoreSlug.Aldi,
                BrandColor = "#00005F",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6d/Aldi_Nord_2017.svg/800px-Aldi_Nord_2017.svg.png",
                IsActive = true,
                WebsiteUrl = "https://www.aldi.hu"
            };
            aldi.Locations.Add(new StoreLocation
            {
                Address = "Váci út 15.",
                City = "Budapest",
                ZipCode = "1134",
                Coordinates = new Point(19.0571, 47.5189) { SRID = 4326 }
            });

            await context.Stores.AddRangeAsync(lidl, spar, aldi);
            await context.SaveChangesAsync();

            // 4. TERMÉKEK
            Console.WriteLine(">>> TERMÉKEK LÉTREHOZÁSA... <<<");

            var milk = new Product
            {
                Name = "Pilos Tej 2,8%",
                NormalizedName = "pilos tej 2,8%",
                Quantity = 1,
                UnitType = ProductUnit.L,
                IsIngredient = true,
                CategoryId = catDairy.Id
            };
            var butter = new Product
            {
                Name = "Rama Margarin",
                NormalizedName = "rama margarin",
                Quantity = 250,
                UnitType = ProductUnit.g,
                IsIngredient = true,
                CategoryId = catDairy.Id
            };
            var yogurt = new Product
            {
                Name = "Danone Joghurt Eper",
                NormalizedName = "danone joghurt eper",
                Quantity = 125,
                UnitType = ProductUnit.g,
                IsIngredient = false,
                CategoryId = catDairyYogurt.Id
            };
            var cheese = new Product
            {
                Name = "Trappista Sajt",
                NormalizedName = "trappista sajt",
                Quantity = 1,
                UnitType = ProductUnit.Kg,
                IsIngredient = true,
                CategoryId = catDairyCheese.Id
            };
            var bread = new Product
            {
                Name = "Félbarna Kenyér",
                NormalizedName = "felbarna kenyer",
                Quantity = 500,
                UnitType = ProductUnit.g,
                IsIngredient = false,
                CategoryId = catBakery.Id
            };
            var chicken = new Product
            {
                Name = "Csirkemell Filé",
                NormalizedName = "csirkemell file",
                Quantity = 1,
                UnitType = ProductUnit.Kg,
                IsIngredient = true,
                CategoryId = catMeat.Id
            };

            await context.Products.AddRangeAsync(milk, butter, yogurt, cheese, bread, chicken);
            await context.SaveChangesAsync();

            // 5. ÁRAK
            Console.WriteLine(">>> ÁRAK HOZZÁRENDELÉSE... <<<");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var in14Days = today.AddDays(14);
            var yesterday = today.AddDays(-1);

            await context.ProductPrices.AddRangeAsync(
                // TEJ - mindhárom boltban, Lidl-ben akciós
                new ProductPrice { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(249, "HUF"), OriginalPrice = new Money(299, "HUF"), IsOnSale = true, DiscountPercent = 17, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow, UnitPriceAmount = 249 },
                new ProductPrice { ProductId = milk.Id, StoreId = spar.Id, Price = new Money(349, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow, UnitPriceAmount = 349 },
                new ProductPrice { ProductId = milk.Id, StoreId = aldi.Id, Price = new Money(269, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow, UnitPriceAmount = 269 },

                // MARGARIN - két boltban, Spar-ban akciós
                new ProductPrice { ProductId = butter.Id, StoreId = lidl.Id, Price = new Money(399, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = butter.Id, StoreId = spar.Id, Price = new Money(449, "HUF"), OriginalPrice = new Money(499, "HUF"), IsOnSale = true, DiscountPercent = 10, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // JOGHURT - Lidl-ben LEJÁRT akció (ValidTo = yesterday), Aldi-ban aktív
                // Ez teszteli az onlyActive=true szűrést!
                new ProductPrice { ProductId = yogurt.Id, StoreId = lidl.Id, Price = new Money(199, "HUF"), OriginalPrice = new Money(249, "HUF"), IsOnSale = true, DiscountPercent = 20, Source = PriceSource.Manual, ValidFrom = today.AddDays(-14), ValidTo = yesterday, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = yogurt.Id, StoreId = aldi.Id, Price = new Money(219, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // SAJT - Spar és Aldi, Aldi-ban akciós
                new ProductPrice { ProductId = cheese.Id, StoreId = spar.Id, Price = new Money(2490, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = cheese.Id, StoreId = aldi.Id, Price = new Money(1990, "HUF"), OriginalPrice = new Money(2490, "HUF"), IsOnSale = true, DiscountPercent = 20, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // KENYÉR - Lidl és Spar
                new ProductPrice { ProductId = bread.Id, StoreId = lidl.Id, Price = new Money(299, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = bread.Id, StoreId = spar.Id, Price = new Money(349, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // CSIRKEMELL - Aldi és Lidl-ben akciós
                new ProductPrice { ProductId = chicken.Id, StoreId = aldi.Id, Price = new Money(1799, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = chicken.Id, StoreId = lidl.Id, Price = new Money(1599, "HUF"), OriginalPrice = new Money(1999, "HUF"), IsOnSale = true, DiscountPercent = 20, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow }
            );
            await context.SaveChangesAsync();

            // 6. ÁRTÖRTÉNET - néhány historikus adat
            Console.WriteLine(">>> ÁRTÖRTÉNET LÉTREHOZÁSA... <<<");

            await context.PriceHistories.AddRangeAsync(
                new PriceHistory { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(319, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddMonths(-2) },
                new PriceHistory { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(299, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddMonths(-1) },
                new PriceHistory { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(249, "HUF"), RecordedAt = DateTimeOffset.UtcNow },
                new PriceHistory { ProductId = chicken.Id, StoreId = lidl.Id, Price = new Money(1999, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddMonths(-1) },
                new PriceHistory { ProductId = chicken.Id, StoreId = lidl.Id, Price = new Money(1599, "HUF"), RecordedAt = DateTimeOffset.UtcNow }
            );
            await context.SaveChangesAsync();

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS SIKERES! ---");
        }
    }
}