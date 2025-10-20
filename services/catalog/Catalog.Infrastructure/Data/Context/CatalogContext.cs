using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data.Context
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public IMongoCollection<ProductBrand> Brands { get; }

        public IMongoCollection<ProductType> Types { get; }

        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
            var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);

            Brands = database.GetCollection<ProductBrand>(configuration["DatabaseSettings:BrandsCollection"]);
            Types = database.GetCollection<ProductType>(configuration["DatabaseSettings:TypesCollection"]);
            Products = database.GetCollection<Product>(configuration["DatabaseSettings:ProductsCollection"]);

            _ =  BrandContextSeed.SeedAsync(Brands);
           _= TypeContextSeed.SeedAsync(Types);
           _= CatalogContextSeed.SeedAsync(Products);
        }
    }
}
