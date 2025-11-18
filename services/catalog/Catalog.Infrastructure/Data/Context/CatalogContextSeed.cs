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
    public class CatalogContextSeed
    {
        public static async Task SeedAsync(IMongoCollection<Product> productCollection)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "products.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"this File Seed is Not Exist : {filePath}");
                return;
            }

            var productData = await File.ReadAllTextAsync(filePath);
            var products = JsonSerializer.Deserialize<List<Product>>(productData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (products == null || !products.Any()) return;

            var bulkOps = new List<WriteModel<Product>>();

            foreach (var product in products)
            {
                var filter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
                var update = Builders<Product>.Update
                    .SetOnInsert(x => x.Id, product.Id); // عشان يحط الـ Id لو جديد

                var upsert = new ReplaceOneModel<Product>(filter, product) { IsUpsert = true };
                bulkOps.Add(upsert);
            }

            await productCollection.BulkWriteAsync(bulkOps);
            Console.WriteLine($"Seeded/Upserted {products.Count} products successfully.");
        }
    }
}
