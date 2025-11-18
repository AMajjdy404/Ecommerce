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
    public class TypeContextSeed
    {
        public static async Task SeedAsync(IMongoCollection<ProductType> typeCollection)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "types.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"this File Seed is Not Exist : {filePath}");
                return;
            }

            var typeData = await File.ReadAllTextAsync(filePath);
            var types = JsonSerializer.Deserialize<List<ProductType>>(typeData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (types == null || !types.Any()) return;

            var bulkOps = new List<WriteModel<ProductType>>();

            foreach (var type in types)
            {
                var filter = Builders<ProductType>.Filter.Eq(x => x.Id, type.Id);
                var upsert = new ReplaceOneModel<ProductType>(filter, type) { IsUpsert = true };
                bulkOps.Add(upsert);
            }

            await typeCollection.BulkWriteAsync(bulkOps);
            Console.WriteLine($"Seeded/Upserted {types.Count} types successfully.");
        }
    }
}
