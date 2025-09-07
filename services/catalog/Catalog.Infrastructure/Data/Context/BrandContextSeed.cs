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
            var hasBrands = await brandCollection.Find(_ => true).AnyAsync();
            if (hasBrands)
                return;
            var filePath = Path.Combine("Data", "DataSeed", "brands.js");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"this File Seed is Not Exist : {filePath}");
                return;
            }
            var brandData = await File.ReadAllTextAsync(filePath);
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
            if (brands?.Any() == true)
                await brandCollection.InsertManyAsync(brands);

        }
    }
}
