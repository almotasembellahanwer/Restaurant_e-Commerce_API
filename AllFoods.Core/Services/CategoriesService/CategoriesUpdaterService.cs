using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.ServiceContracts.ICategoriesService;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AllFoods.Core.Services.CategoriesService
{
    public class CategoriesUpdaterService : ICategoriesUpdaterService
    {
        private readonly IMapper _mapper;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ILogger<CategoriesUpdaterService> _logger;

        public CategoriesUpdaterService(IMapper mapper, ICategoriesRepository categoriesRepository, ILogger<CategoriesUpdaterService> logger)
        {
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest categoryUpdateRequest)
        {
            _logger.LogInformation("Category ID from categoryUpdateRequest : {categoryID}", categoryUpdateRequest.CategoryID);
            if (categoryUpdateRequest is null)
                throw new ArgumentNullException($" {nameof(categoryUpdateRequest)} should not be null");

            if (categoryUpdateRequest.CategoryName is null)
                throw new ArgumentException($" {nameof(categoryUpdateRequest.CategoryName)} should not be null");
            Category toCategoryObj = categoryUpdateRequest.ToCategory();
            _logger.LogInformation("Category ID from Category : {categoryID}",toCategoryObj.CategoryID);
            Category result = await _categoriesRepository.UpdateCategory(toCategoryObj);
            CategoryResponse categoryResponse = _mapper.Map<CategoryResponse>(result);
            _logger.LogInformation("Category from CategoryResponse : {categoryID}", toCategoryObj.CategoryID);

            return categoryResponse;
        }
    }
}
