using AllFoods.Core.Domain.Entities;

namespace AllFoods.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Representing data logic for the Category entity.
    /// </summary>
    public interface ICategoriesRepository
    {
        /// <summary>
        /// Add category to the collection
        /// </summary>
        /// <param name="category">Category to add</param>
        /// <returns>Returns category that has been added</returns>
        Task<Category> AddCategory(Category category);
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>Returns a list of categories</returns>
        Task<List<Category>> GetAllCategories();
        /// <summary>
        /// Get category based on the categoryID
        /// </summary>
        /// <param name="categoryID">categoryID to Get the category based on it</param>
        /// <returns>Returns category based on categoryID</returns>
        Task<Category?> GetCategoryByCategoryID(Guid? categoryID);
        /// <summary>
        /// Get category based on the categoryName
        /// </summary>
        /// <param name="categoryName">categoryName to Get the category based on it</param>
        /// <returns>Returns category based on categoryName</returns>
        Task<Category?> GetCategoryByCategoryName(string? categoryName);
        /// <summary>
        /// Update category based on category information
        /// </summary>
        /// <param name="category">category to update</param>
        /// <returns>Returns category that has been updated</returns>

        Task<Category> UpdateCategory(Category category);
        /// <summary>
        /// Delete category based on categoryID
        /// </summary>
        /// <param name="categoryID">categoryID to delete category based on it</param>
        /// <returns>Returns true if category deleted otherwise not</returns>

        Task<bool> DeleteCategory(Guid? categoryID);

    }
}
