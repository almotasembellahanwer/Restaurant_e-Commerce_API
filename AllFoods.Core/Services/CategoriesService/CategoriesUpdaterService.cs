using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.ServiceContracts.ICategoriesService;
using AutoMapper;

namespace AllFoods.Core.Services.CategoriesService
{
    public class CategoriesUpdaterService : ICategoriesUpdaterService
    {
        private readonly IMapper _mapper;
        private readonly ICategoriesRepository _categoriesRepository;


        public CategoriesUpdaterService(IMapper mapper, ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest categoryUpdateRequest)
        {
            if (categoryUpdateRequest is null)
                throw new ArgumentNullException($" {nameof(categoryUpdateRequest)} should not be null");

            if (categoryUpdateRequest.CategoryName is null)
                throw new ArgumentException($" {nameof(categoryUpdateRequest.CategoryName)} should not be null");
            Category paramToCategoryObj = categoryUpdateRequest.ToCategory();
            Category result = await _categoriesRepository.UpdateCategory(paramToCategoryObj);

            return _mapper.Map<CategoryResponse>(result);
        }
    }
}
