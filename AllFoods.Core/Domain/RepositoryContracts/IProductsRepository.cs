using AllFoods.Core.Domain.Entities;
using System.Linq.Expressions;

namespace AllFoods.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Representing data logic for the Product entity.
    /// </summary>
    public interface IProductsRepository
    {
        /// <summary>
        /// Add product to the collection
        /// </summary>
        /// <param name="product">Product to add</param>
        /// <returns>Returns product that has been added</returns>
        Task<Product> AddProduct(Product product);
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Returns a list of products</returns>
        Task<List<Product>> GetAllProducts();
        /// <summary>
        /// Get product based on the productID
        /// </summary>
        /// <param name="productID">productID to Get the product based on it</param>
        /// <returns>Returns product based on productID</returns>
        Task<Product?> GetProductByProductID(Guid? productID);
        /// <summary>
        /// Get List of products that match the predicate
        /// </summary>
        /// <param name="predicate">predicate to Get List of products that match</param>
        /// <returns>Returns list of products based on predicate</returns>
        Task<List<Product>> GetFilteredProduct(Expression<Func<Product,bool>> predicate);

        /// <summary>
        /// Update product that retrieved from database
        /// </summary>
        /// <param name="product">product to update</param>
        /// <returns>Returns Product that has been updated</returns>
        Task<Product> UpdateProduct(Product product);

        /// <summary>
        /// Delete Product based on productID
        /// </summary>
        /// <param name="productID">productID to delete</param>
        /// <returns>Returns true if product deleted otherwise false</returns>
        Task<bool> DeleteProductByProductID(Guid? productID);



    }
}
