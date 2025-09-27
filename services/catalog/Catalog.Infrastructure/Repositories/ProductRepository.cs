using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Context;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{

    public class ProductRepository : IProductRepository, ITypeRepository, IBrandRepository
    {
        private readonly CatalogContext _context;

        public ProductRepository(CatalogContext context)
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
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.Find(p=> true).ToListAsync();
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
    }
}
