using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.ServiceContracts.ICategoriesService;
using AutoMapper;

namespace AllFoods.Core.Services.CategoriesService
{
    public class CategoriesAdderService : ICategoriesAdderService
    {
        private readonly IMapper _mapper;
        private readonly ICategoriesRepository _categoriesRepository;


        public CategoriesAdderService(IMapper mapper, ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponse> AddCategory(CategoryAddRequest? categoryAddRequest)
        {
            if (categoryAddRequest is null)
                throw new ArgumentNullException($" {nameof(categoryAddRequest)} should not be null");

            if (categoryAddRequest.CategoryName is null)
                throw new ArgumentException($" {nameof(categoryAddRequest.CategoryName)} should not be null");

            Category? categoryName = await _categoriesRepository.GetCategoryByCategoryName(categoryAddRequest.CategoryName);
            if (categoryName is not null)
            {
                throw new ArgumentException($"{nameof(categoryName)} should not be duplicated");
            }
            Category paramToCategoryObj = categoryAddRequest.ToCategory();
            Category result = await _categoriesRepository.AddCategory(paramToCategoryObj);

            return _mapper.Map<CategoryResponse>(result);
        }
    }
}
