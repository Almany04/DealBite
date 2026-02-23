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
            await context.Database.MigrateAsync();

            // 1. CSAK AKKOR SEEDELÜNK, HA ÜRES A DATABASE
            if (await context.Categories.AnyAsync())
            {
                Console.WriteLine("--- AZ ADATBÁZIS MÁR TARTALMAZ ADATOKAT, SEEDING KIHAGYVA ---");
                return;
            }

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS INDUL ---");

            //// 1. TAKARÍTÁS
            //Console.WriteLine(">>> RÉGI ADATOK TÖRLÉSE... <<<");

            //if (context.ProductPrices.Any()) context.ProductPrices.RemoveRange(context.ProductPrices);
            //if (context.ShoppingListItems.Any()) context.ShoppingListItems.RemoveRange(context.ShoppingListItems);
            //if (context.ShoppingLists.Any()) context.ShoppingLists.RemoveRange(context.ShoppingLists);
            //if (context.RecipeIngredients.Any()) context.RecipeIngredients.RemoveRange(context.RecipeIngredients);
            //if (context.Recipes.Any()) context.Recipes.RemoveRange(context.Recipes);
            //if (context.PriceHistories.Any()) context.PriceHistories.RemoveRange(context.PriceHistories);
            //if (context.Products.Any()) context.Products.RemoveRange(context.Products);
            //if (context.StoreLocations.Any()) context.StoreLocations.RemoveRange(context.StoreLocations);
            //if (context.Stores.Any()) context.Stores.RemoveRange(context.Stores);
            //if (context.Categories.Any()) context.Categories.RemoveRange(context.Categories);

            //await context.SaveChangesAsync();

            // 2. KATEGÓRIÁK
            Console.WriteLine(">>> KATEGÓRIÁK LÉTREHOZÁSA... <<<");

            var catDairy = new Category { Name = "Tejtermékek", Slug = "tejtermekek" };
            var catBakery = new Category { Name = "Pékáru", Slug = "pekaru" };
            var catMeat = new Category { Name = "Húsok", Slug = "husok" };
            var catGrain = new Category { Name = "Szárazáruk", Slug = "szarazaruk" };
            var catEgg = new Category { Name = "Tojás", Slug = "tojas" };
            var catDairyYogurt = new Category { Name = "Joghurtok", Slug = "joghurtok", ParentCategory = catDairy };
            var catDairyCheese = new Category { Name = "Sajtok", Slug = "sajtok", ParentCategory = catDairy };

            await context.Categories.AddRangeAsync(catDairy, catBakery, catMeat, catGrain, catEgg, catDairyYogurt, catDairyCheese);
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

            // Tej: mindhárom boltban, jó history → teszteli Excellent/Average/Expensive
            var milk = new Product
            {
                Name = "Pilos Tej 2,8%",
                NormalizedName = "pilos tej 2,8%",
                Quantity = 1,
                UnitType = ProductUnit.L,
                IsIngredient = true,
                CategoryId = catDairy.Id
            };

            // Margarin: két boltban (Lidl, Spar), van history
            var butter = new Product
            {
                Name = "Rama Margarin",
                NormalizedName = "rama margarin",
                Quantity = 250,
                UnitType = ProductUnit.g,
                IsIngredient = true,
                CategoryId = catDairy.Id
            };

            // Joghurt: két boltban (Lidl lejárt, Aldi aktív)
            var yogurt = new Product
            {
                Name = "Danone Joghurt Eper",
                NormalizedName = "danone joghurt eper",
                Quantity = 125,
                UnitType = ProductUnit.g,
                IsIngredient = false,
                CategoryId = catDairyYogurt.Id
            };

            // Sajt: két boltban (Spar, Aldi), van history
            var cheese = new Product
            {
                Name = "Trappista Sajt",
                NormalizedName = "trappista sajt",
                Quantity = 1,
                UnitType = ProductUnit.Kg,
                IsIngredient = true,
                CategoryId = catDairyCheese.Id
            };

            // Kenyér: mindhárom boltban
            var bread = new Product
            {
                Name = "Félbarna Kenyér",
                NormalizedName = "felbarna kenyer",
                Quantity = 500,
                UnitType = ProductUnit.g,
                IsIngredient = false,
                CategoryId = catBakery.Id
            };

            // Csirkemell: két boltban (Aldi, Lidl), van history
            var chicken = new Product
            {
                Name = "Csirkemell Filé",
                NormalizedName = "csirkemell file",
                Quantity = 1,
                UnitType = ProductUnit.Kg,
                IsIngredient = true,
                CategoryId = catMeat.Id
            };

            // Tojás: mindhárom boltban, DE NINCS history → null evaluation teszt
            var egg = new Product
            {
                Name = "Tojás 10 db",
                NormalizedName = "tojas 10 db",
                Quantity = 10,
                UnitType = ProductUnit.Db,
                IsIngredient = true,
                CategoryId = catEgg.Id
            };

            // Rizs: két boltban, CSAK RÉGI history (>8 hét) → sliding window teszt
            var rice = new Product
            {
                Name = "Basmati Rizs",
                NormalizedName = "basmati rizs",
                Quantity = 1,
                UnitType = ProductUnit.Kg,
                IsIngredient = true,
                CategoryId = catGrain.Id
            };

            await context.Products.AddRangeAsync(milk, butter, yogurt, cheese, bread, chicken, egg, rice);
            await context.SaveChangesAsync();

            // 5. ÁRAK
            Console.WriteLine(">>> ÁRAK HOZZÁRENDELÉSE... <<<");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var in14Days = today.AddDays(14);
            var yesterday = today.AddDays(-1);

            await context.ProductPrices.AddRangeAsync(
                // ============================================================
                // TEJ - mindhárom boltban
                // History median: ~299 HUF
                // Lidl 249 → deviation +16.7% → Excellent
                // Spar 299 → deviation  0.0% → Average
                // Aldi 349 → deviation -16.7% → Expensive
                // ============================================================
                new ProductPrice { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(249, "HUF"), OriginalPrice = new Money(299, "HUF"), IsOnSale = true, DiscountPercent = 17, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow, UnitPriceAmount = 249 },
                new ProductPrice { ProductId = milk.Id, StoreId = spar.Id, Price = new Money(299, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow, UnitPriceAmount = 299 },
                new ProductPrice { ProductId = milk.Id, StoreId = aldi.Id, Price = new Money(349, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow, UnitPriceAmount = 349 },

                // ============================================================
                // MARGARIN - Lidl és Spar
                // History median: ~449 HUF
                // Lidl 399 → deviation +11.1% → Excellent
                // Spar 449 → deviation  0.0% → Average
                // ============================================================
                new ProductPrice { ProductId = butter.Id, StoreId = lidl.Id, Price = new Money(399, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = butter.Id, StoreId = spar.Id, Price = new Money(449, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // ============================================================
                // JOGHURT - Lidl LEJÁRT akció, Aldi aktív
                // History median: ~229 HUF
                // Lidl 199 (LEJÁRT!) → nem kéne megjelennie aktív query-ben
                // Aldi 219 → deviation +4.4% → Average
                // ============================================================
                new ProductPrice { ProductId = yogurt.Id, StoreId = lidl.Id, Price = new Money(199, "HUF"), OriginalPrice = new Money(249, "HUF"), IsOnSale = true, DiscountPercent = 20, Source = PriceSource.Manual, ValidFrom = today.AddDays(-14), ValidTo = yesterday, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = yogurt.Id, StoreId = aldi.Id, Price = new Money(219, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // ============================================================
                // SAJT - Spar és Aldi
                // History median: ~2490 HUF
                // Spar 2490 → deviation  0.0% → Average
                // Aldi 1990 → deviation +20.1% → Excellent
                // ============================================================
                new ProductPrice { ProductId = cheese.Id, StoreId = spar.Id, Price = new Money(2490, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = cheese.Id, StoreId = aldi.Id, Price = new Money(1990, "HUF"), OriginalPrice = new Money(2490, "HUF"), IsOnSale = true, DiscountPercent = 20, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // ============================================================
                // KENYÉR - mindhárom boltban
                // History median: ~329 HUF
                // Lidl 299 → deviation +9.1%  → Average (pont a határon belül!)
                // Spar 349 → deviation -6.1%  → Average
                // Aldi 319 → deviation +3.0%  → Average
                // ============================================================
                new ProductPrice { ProductId = bread.Id, StoreId = lidl.Id, Price = new Money(299, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = bread.Id, StoreId = spar.Id, Price = new Money(349, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = bread.Id, StoreId = aldi.Id, Price = new Money(319, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // ============================================================
                // CSIRKEMELL - Aldi és Lidl
                // History median: ~1899 HUF
                // Lidl 1599 → deviation +15.8% → Excellent
                // Aldi 1899 → deviation  0.0%  → Average
                // ============================================================
                new ProductPrice { ProductId = chicken.Id, StoreId = aldi.Id, Price = new Money(1899, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = chicken.Id, StoreId = lidl.Id, Price = new Money(1599, "HUF"), OriginalPrice = new Money(1999, "HUF"), IsOnSale = true, DiscountPercent = 20, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // ============================================================
                // TOJÁS - mindhárom boltban, NINCS price history!
                // → DeviationPercent: null, PriceEvaluation: null
                // ============================================================
                new ProductPrice { ProductId = egg.Id, StoreId = lidl.Id, Price = new Money(599, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = egg.Id, StoreId = spar.Id, Price = new Money(649, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = egg.Id, StoreId = aldi.Id, Price = new Money(579, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },

                // ============================================================
                // RIZS - Lidl és Spar, CSAK RÉGI history (>8 hét)
                // → Sliding window kiszűri → DeviationPercent: null
                // ============================================================
                new ProductPrice { ProductId = rice.Id, StoreId = lidl.Id, Price = new Money(899, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow },
                new ProductPrice { ProductId = rice.Id, StoreId = spar.Id, Price = new Money(949, "HUF"), IsOnSale = false, Source = PriceSource.Manual, ValidFrom = today, ValidTo = in14Days, LastScrapedAt = DateTimeOffset.UtcNow }
            );
            await context.SaveChangesAsync();

            // 6. ÁRTÖRTÉNET
            Console.WriteLine(">>> ÁRTÖRTÉNET LÉTREHOZÁSA... <<<");

            await context.PriceHistories.AddRangeAsync(
                // ============================================================
                // TEJ - 5 bejegyzés az elmúlt 8 héten belül
                // Értékek: 279, 289, 299, 309, 319 → Median: 299
                // ============================================================
                new PriceHistory { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(319, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-50) },
                new PriceHistory { ProductId = milk.Id, StoreId = spar.Id, Price = new Money(309, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-40) },
                new PriceHistory { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(299, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-30) },
                new PriceHistory { ProductId = milk.Id, StoreId = aldi.Id, Price = new Money(289, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-20) },
                new PriceHistory { ProductId = milk.Id, StoreId = lidl.Id, Price = new Money(279, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-10) },

                // ============================================================
                // MARGARIN - 3 bejegyzés
                // Értékek: 429, 449, 469 → Median: 449
                // ============================================================
                new PriceHistory { ProductId = butter.Id, StoreId = lidl.Id, Price = new Money(469, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-45) },
                new PriceHistory { ProductId = butter.Id, StoreId = spar.Id, Price = new Money(449, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-30) },
                new PriceHistory { ProductId = butter.Id, StoreId = lidl.Id, Price = new Money(429, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-15) },

                // ============================================================
                // JOGHURT - 3 bejegyzés
                // Értékek: 219, 229, 239 → Median: 229
                // ============================================================
                new PriceHistory { ProductId = yogurt.Id, StoreId = aldi.Id, Price = new Money(239, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-42) },
                new PriceHistory { ProductId = yogurt.Id, StoreId = lidl.Id, Price = new Money(229, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-28) },
                new PriceHistory { ProductId = yogurt.Id, StoreId = aldi.Id, Price = new Money(219, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-14) },

                // ============================================================
                // SAJT - 3 bejegyzés
                // Értékek: 2390, 2490, 2590 → Median: 2490
                // ============================================================
                new PriceHistory { ProductId = cheese.Id, StoreId = spar.Id, Price = new Money(2590, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-48) },
                new PriceHistory { ProductId = cheese.Id, StoreId = aldi.Id, Price = new Money(2490, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-32) },
                new PriceHistory { ProductId = cheese.Id, StoreId = spar.Id, Price = new Money(2390, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-16) },

                // ============================================================
                // KENYÉR - 3 bejegyzés
                // Értékek: 309, 329, 349 → Median: 329
                // ============================================================
                new PriceHistory { ProductId = bread.Id, StoreId = lidl.Id, Price = new Money(349, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-42) },
                new PriceHistory { ProductId = bread.Id, StoreId = spar.Id, Price = new Money(329, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-28) },
                new PriceHistory { ProductId = bread.Id, StoreId = lidl.Id, Price = new Money(309, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-14) },

                // ============================================================
                // CSIRKEMELL - 3 bejegyzés
                // Értékek: 1799, 1899, 1999 → Median: 1899
                // ============================================================
                new PriceHistory { ProductId = chicken.Id, StoreId = lidl.Id, Price = new Money(1999, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-40) },
                new PriceHistory { ProductId = chicken.Id, StoreId = aldi.Id, Price = new Money(1899, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-25) },
                new PriceHistory { ProductId = chicken.Id, StoreId = lidl.Id, Price = new Money(1799, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-10) },

                // ============================================================
                // TOJÁS - NINCS history! (szándékosan kihagyva)
                // ============================================================

                // ============================================================
                // RIZS - CSAK RÉGI history (>56 nap, sliding window-on kívül!)
                // Értékek: 799, 849, 899 → DE a window kiszűri!
                // ============================================================
                new PriceHistory { ProductId = rice.Id, StoreId = lidl.Id, Price = new Money(899, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-90) },
                new PriceHistory { ProductId = rice.Id, StoreId = spar.Id, Price = new Money(849, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-75) },
                new PriceHistory { ProductId = rice.Id, StoreId = lidl.Id, Price = new Money(799, "HUF"), RecordedAt = DateTimeOffset.UtcNow.AddDays(-60) }
            );
            await context.SaveChangesAsync();

            Console.WriteLine("--- ADATBÁZIS INITIALIZÁLÁS SIKERES! ---");
        }
    }
}