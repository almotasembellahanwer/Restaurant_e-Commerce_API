using AllFoods.Core.DTO.CategoyDTO;


namespace AllFoods.Core.ServiceContracts.ICategoriesService
{
    public interface ICategoriesDeleterService
    {
        Task<bool> DeleteCategory(Guid? categoryID);
    }
}
