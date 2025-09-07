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
            var hasProducts = await productCollection.Find(_ => true).AnyAsync();
            if (hasProducts)
                return;
            var filePath = Path.Combine("Data", "DataSeed", "products.js");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"this File Seed is Not Exist : {filePath}");
                return;
            }
            var productData = await File.ReadAllTextAsync(filePath);
            var products = JsonSerializer.Deserialize<List<Product>>(productData);
            if (products?.Any() == true)
                await productCollection.InsertManyAsync(products);

        }
    }
}
