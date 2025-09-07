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
            var hastypes = await typeCollection.Find(_ => true).AnyAsync();
            if (hastypes)
                return;
            var filePath = Path.Combine("Data", "DataSeed", "types.js");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"this File Seed is Not Exist : {filePath}");
                return;
            }
            var typeData = await File.ReadAllTextAsync(filePath);
            var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);
            if (types?.Any() == true)
                await typeCollection.InsertManyAsync(types);

        }
    }
}
