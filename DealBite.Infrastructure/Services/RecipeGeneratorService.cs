using DealBite.Application.Common.Models;
using DealBite.Application.Common.Settings;
using DealBite.Application.Interfaces.Services;
using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace DealBite.Infrastructure.Services
{
    public class RecipeGeneratorService : IRecipeGeneratorService
    {
        private readonly GeminiSettings _settings;
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RecipeGeneratorService(IOptions<GeminiSettings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
        }

        public async Task<List<GeneratedRecipeData>> GenerateRecipesAsync(
            List<ProductPrice> onSaleProducts,
            ProductSegment segment,
            int count = 10)
        {
            var prompt = BuildPrompt(onSaleProducts, segment, count);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    temperature = 0.7
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key={_settings.ApiKey}";

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            var recipes = ParseGeminiResponse(responseJson);

            return recipes;
        }

        private static string BuildPrompt(List<ProductPrice> onSaleProducts, ProductSegment segment, int count)
        {
            var productListBuilder = new StringBuilder();

            foreach (var p in onSaleProducts)
            {
                productListBuilder.AppendLine(
                    $"- ProductId: \"{p.ProductId}\", " +
                    $"Név: \"{p.Product?.Name ?? "Ismeretlen"}\", " +
                    $"Ár: {p.Price.Amount} HUF, " +
                    $"Bolt: \"{p.Store?.Name ?? "Ismeretlen"}\", " +
                    $"Kategória: \"{p.Product?.Category?.Name ?? "Ismeretlen"}\", " +
                    $"Mennyiség: {p.Product?.Quantity} {p.Product?.UnitType}"
                );
            }

            var segmentDescription = segment switch
            {
                ProductSegment.Gazdasagos => "gazdaságos (olcsó, saját márkás termékek)",
                ProductSegment.Standard => "standard (ismert márkák, átlagos árkategória)",
                ProductSegment.Premium => "prémium (bio, kézműves, specialitás)",
                _ => "bármilyen árkategória"
            };

            return $@"Te egy magyar élelmiszer-receptgenerátor AI vagy a DealBite nevű bevásárlás-optimalizáló alkalmazáshoz.

            Az alábbi termékek jelenleg AKCIÓSAK magyar élelmiszer-boltokban:

                {productListBuilder}

            Feladat: Generálj pontosan {count} magyar receptet, amelyek a lehető legjobban kihasználják ezeket az akciós termékeket.

            Szabályok:
            1. Minden recept használjon minél több akciós terméket FŐ HOZZÁVALÓKÉNT
            2. Rangsorold a recepteket: az #1 recept használja a legtöbb akciós terméket / hozza a legnagyobb megtakarítást
            3. Minden hozzávalót listázz (akciós ÉS nem akciós, pl. só, olaj, fűszerek)
            4. Akciós hozzávalóknál KÖTELEZŐEN használd a fenti listából a pontos ProductId-t (GUID formátumban)
            5. Nem akciós hozzávalóknál (só, fűszer, olaj stb.) a ProductId legyen null
            6. A receptek legyenek gyakorlatias, hétköznapi magyar ételek
            7. Árkategória: {segmentDescription} – a receptek illeszkedjenek ehhez
            8. A UnitType mező értékei CSAK ezek lehetnek: Kg, Db, L, Dl, Ml, Csomag, Dkg, g

            KIZÁRÓLAG az alábbi JSON struktúrában válaszolj, semmilyen extra szöveget ne írj:

            [
                {{
                    ""title"": ""Recept neve"",
                    ""description"": ""Rövid leírás, 1-2 mondat"",
                    ""prepTimeMinutes"": 30,
                    ""servings"": 4,
                    ""ingredients"": [
                    {{
                        ""ingredientName"": ""Akciós hozzávaló neve"",
                        ""amount"": 1.0,
                        ""unitType"": ""Kg"",
                        ""productId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6""
                    }},
                {{
                   ""ingredientName"": ""Só"",
                   ""amount"": 1.0,
                   ""unitType"": ""Dkg"",
                   ""productId"": null
                }}
           ],
            ""steps"": [
                {{
                    ""stepNumber"": 1,
                    ""instruction"": ""Első lépés leírása""
                }}
           ]
                }}
          ]";
        }

        private static List<GeneratedRecipeData> ParseGeminiResponse(string responseJson)
        {

            using var doc = JsonDocument.Parse(responseJson);

            var textContent = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrEmpty(textContent))
            {
                throw new InvalidOperationException("A Gemini API üres választ adott.");
            }

            var recipes = JsonSerializer.Deserialize<List<GeneratedRecipeData>>(textContent, _jsonOptions);

            if (recipes == null || recipes.Count == 0)
            {
                throw new InvalidOperationException("A Gemini API nem adott vissza recepteket.");
            }

            return recipes;
        }
    }
}