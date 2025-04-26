using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.CategoyDTO;


namespace AllFoods.Core.ServiceContracts.ICategoriesService
{
    public interface ICategoriesUpdaterService
    {

        Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest categoryUpdateRequest);
    }
}
