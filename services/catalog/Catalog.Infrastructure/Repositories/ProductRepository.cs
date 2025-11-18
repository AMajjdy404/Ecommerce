using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data.Context;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{

    public class ProductRepository : IProductRepository, ITypeRepository, IBrandRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateProduct(Product product)
        {
             await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProduct(string id)
        {
           var deletedProduct = await _context.Products.DeleteOneAsync(id);
           return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }
        public async Task<Pagination<Product>> GetAllProducts(CatalogParamSpec catalogParamSpec)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(catalogParamSpec.Search))
            {
                filter = filter & builder.Where(p => p.Name.ToLower().Contains(catalogParamSpec.Search.ToLower()));
            }
            if (!string.IsNullOrEmpty(catalogParamSpec.BrandId))
            {
                var brandFilter = builder.Eq(p => p.ProductBrand.Id, catalogParamSpec.BrandId);
                filter &= brandFilter;
            }
            if (!string.IsNullOrEmpty(catalogParamSpec.TypeId))
            {
                var typeFilter = builder.Eq(p => p.ProductType.Id, catalogParamSpec.TypeId);
                filter &= typeFilter;
            }
            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(catalogParamSpec, filter);

            return new Pagination<Product>()
            {
                PageIndex = catalogParamSpec.PageIndex,
                PageSize = catalogParamSpec.PageSize,
                TotalItems = (int) totalItems,
                Data = data
            };
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await _context.Brands.Find(p => true).ToListAsync();

        }


        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await _context.Types.Find(p => true).ToListAsync();

        }

        public async Task<Product> GetById(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandName(string brandName)
        {
            return await _context.Products.Find(p=>p.ProductBrand.Name == brandName).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            return await _context.Products.Find(p =>p.Name == name).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        private async Task<IReadOnlyList<Product>> DataFilter(CatalogParamSpec spec ,FilterDefinition<Product> filter)
        {
            var sortDef = Builders<Product>.Sort.Ascending("Name");
            if (!string.IsNullOrEmpty(spec.Sort))
            {
                switch (spec.Sort)
                {
                    case "priceAsc":
                        sortDef = Builders<Product>.Sort.Ascending(p => p.Price); 
                        break;
                    case "priceDesc":
                        sortDef = Builders<Product>.Sort.Descending(p => p.Price);
                        break;
                    default:
                        sortDef = Builders<Product>.Sort.Ascending("Name");
                        break;
                }
            }
            return  await _context.Products
                    .Find(filter)
                    .Sort(sortDef)
                    .Skip(spec.PageSize *(spec.PageIndex -1))
                    .Limit(spec.PageSize)
                    .ToListAsync();
        }
    }
}
