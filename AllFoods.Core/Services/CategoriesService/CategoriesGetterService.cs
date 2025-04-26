using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.ServiceContracts.ICategoriesService;
using AutoMapper;

namespace AllFoods.Core.Services.CategoriesService
{
    public class CategoriesGetterService : ICategoriesGetterService
    {

        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;

        public CategoriesGetterService(IMapper mapper, ICategoriesRepository categoriesRepository)
        {
            _mapper = mapper;
            _categoriesRepository = categoriesRepository;
        }

        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            List<Category> categories = await _categoriesRepository.GetAllCategories();
            return _mapper.Map<List<CategoryResponse>>(categories);
        }

        public async Task<CategoryResponse?> GetCategoryByCategoryID(Guid? categoryID)
        {
            // If category id is null return null
            if (categoryID is null)
                return null;
            Category? category = await _categoriesRepository.GetCategoryByCategoryID(categoryID);
            // If category is nul return null
            if (category is null)
                return null;
            return _mapper.Map<CategoryResponse>(category);
        }
    }
}
