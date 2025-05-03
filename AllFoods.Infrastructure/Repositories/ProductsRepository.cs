using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AllFoods.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Product> AddProduct(Product product)
        {
             _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }


        public async Task<List<Product>> GetAllProducts(int pageNumber,int pageSize)
        {
            IQueryable<Product> products = _db.Products
                .Include(temp => temp.Category);
            if(pageSize > 0)
            {
                if(pageSize > 100)
                {
                    pageSize = 100;
                }
                products = products.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }
            return await products.ToListAsync();
        }


        public async Task<Product?> GetProductByProductID(Guid? productID)
        {
            return await _db.Products.FirstOrDefaultAsync(temp=>temp.ProductID == productID);
        }

        public async Task<List<Product>> GetFilteredProduct(Expression<Func<Product, bool>> predicate)
        {
            return await _db.Products.Where(predicate).Include("Category").ToListAsync();
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            Product? matchingProduct = await _db.Products.FirstOrDefaultAsync(temp=>temp.ProductID == product.ProductID);
            if(matchingProduct is null)
            {
                return product;
            }
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return matchingProduct;
        }

        public async Task<bool> DeleteProductByProductID(Guid? productID)
        {
            var productToDelete = _db.Products.Where(temp => temp.ProductID == productID);
            _db.Products.RemoveRange(productToDelete);
            int affectedRows = await _db.SaveChangesAsync();
            return affectedRows > 0;
        }

    }
}
