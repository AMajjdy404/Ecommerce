using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Core.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data.Context
{
    public static class BrandContextSeed
    {
        public static async Task SeedAsync(IMongoCollection<ProductBrand> brandCollection)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "brands.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"this File Seed is Not Exist : {filePath}");
                return;
            }

            var brandData = await File.ReadAllTextAsync(filePath);
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (brands == null || !brands.Any()) return;

            var bulkOps = new List<WriteModel<ProductBrand>>();

            foreach (var brand in brands)
            {
                var filter = Builders<ProductBrand>.Filter.Eq(x => x.Id, brand.Id);
                var upsert = new ReplaceOneModel<ProductBrand>(filter, brand) { IsUpsert = true };
                bulkOps.Add(upsert);
            }

            await brandCollection.BulkWriteAsync(bulkOps);
            Console.WriteLine($"Seeded/Upserted {brands.Count} brands successfully.");
        }
    }
}
