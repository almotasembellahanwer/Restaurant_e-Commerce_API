using AllFoods.Core.DTO.CategoyDTO;


namespace AllFoods.Core.ServiceContracts.ICategoriesService
{
    public interface ICategoriesAdderService
    {
        /// <summary>
        /// Create a new Category
        /// </summary>
        /// <param name="categoryAddRequest">Pass category request as argument</param>
        /// <returns>Return Category response</returns>
        Task<CategoryResponse> AddCategory(CategoryAddRequest? categoryAddRequest);
    }
}
