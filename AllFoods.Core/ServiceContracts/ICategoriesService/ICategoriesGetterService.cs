using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.CategoyDTO;

namespace AllFoods.Core.ServiceContracts.ICategoriesService
{
    public interface ICategoriesGetterService
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>Returns List of CategoryResponse</returns>
        Task<List<CategoryResponse>> GetAllCategories();

        /// <summary>
        /// Get category based on the categoryID
        /// </summary>
        /// <param name="categoryID">categoryID to Get the category based on it</param>
        /// <returns>Returns category based on categoryID</returns>
        Task<CategoryResponse?> GetCategoryByCategoryID(Guid? categoryID);
    }
}
