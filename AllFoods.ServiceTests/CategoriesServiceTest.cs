using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.MappingCofig;
using AllFoods.Core.Services.CategoriesService;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace AllFoods.ServiceTests
{
    public class CategoriesServiceTest
    {
        private readonly CategoriesAdderService _categoriesAdderService;
        private readonly CategoriesGetterService _categoriesGetterService;

        private readonly IFixture _fixture;
        private readonly IMapper _mapper;

        private readonly Mock<ICategoriesRepository> _categoriesRepositoryMock;
        private readonly ICategoriesRepository _categoriesRepository;
        public CategoriesServiceTest()
        {
            _categoriesRepositoryMock = new Mock<ICategoriesRepository>();
            _categoriesRepository = _categoriesRepositoryMock!.Object;

            _fixture = new Fixture();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfig>();
            });
            _mapper = config.CreateMapper();

            _categoriesAdderService = new CategoriesAdderService(_mapper, _categoriesRepository!);
            _categoriesGetterService = new CategoriesGetterService(_mapper, _categoriesRepository!);


            
        }
        // When CategoryAddRequest is null, it should throw ArgumentException
        [Fact]
        public async Task AddCategory_NullCategory_ToBeArgumentNullException()
        {
            // Arrange
            CategoryAddRequest? categoryAddRequest = null;
            Func<Task> action = async () =>
            {
                // Act
                await _categoriesAdderService.AddCategory(categoryAddRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        // When CategoryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCategory_CategoryNameIsNull_ToBeArgumentException()
        {
            // Arrange
            CategoryAddRequest categoryAddRequest = _fixture.Build<CategoryAddRequest>()
                .With(temp => temp.CategoryName, null as string)
                .Create();
            Category category = categoryAddRequest.ToCategory();
            _categoriesRepositoryMock.Setup(temp => temp.AddCategory(It.IsAny<Category>()))
                .ReturnsAsync(category);
            // Act
            Func<Task> action = async () =>
            {
                await _categoriesAdderService.AddCategory(categoryAddRequest);
            };
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        // When CategoryName dublicated, it should throw ArgumentException
        [Fact]
        public async Task AddCategory_DuplicateCategoryName_ToBeArgumentException()
        {
            // Arrange
            CategoryAddRequest categoryAddRequest1 = _fixture.Build<CategoryAddRequest>()
                .With(temp => temp.CategoryName, "Drinks")
                .Create();

            CategoryAddRequest categoryAddRequest2 = _fixture.Build<CategoryAddRequest>()
                .With(temp => temp.CategoryName, "Drinks")
                .Create();

            Category category = categoryAddRequest1.ToCategory();

            _categoriesRepositoryMock.Setup(temp => temp.AddCategory(It.IsAny<Category>()))
                .ReturnsAsync(category);
            _categoriesRepositoryMock.Setup(temp => temp.GetCategoryByCategoryName(It.IsAny<string>()))
                .ReturnsAsync(category);
            // Act
            Func<Task> action = async () =>
            {
                await _categoriesAdderService.AddCategory(categoryAddRequest1);
                await _categoriesAdderService.AddCategory(categoryAddRequest2);
            };
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        //When you supply proper category name, it should insert (add) the category to the existing list of categories
        [Fact]
        public async Task AddCategory_FullCategory_ToBeSuccessful()
        {
            // Arrange
            CategoryAddRequest categoryAddRequest = _fixture.Build<CategoryAddRequest>()
                .Create();

            // Convert to Category
            Category category = categoryAddRequest.ToCategory();

            _categoriesRepositoryMock.Setup(temp => temp.AddCategory(It.IsAny<Category>())).ReturnsAsync(category);

            // Act
            CategoryResponse categoryResponseFromAdd = await _categoriesAdderService.AddCategory(categoryAddRequest);
            category.CategoryID = categoryResponseFromAdd.CategoryID;

            // Assert
            categoryResponseFromAdd.Should().NotBeNull();
            categoryResponseFromAdd.CategoryID.Should().Be(category.CategoryID);

        }

        [Fact]

        public async Task GetAllCategories_EmptyList_ToBeSuccessful()
        {
            // Arrange
            List<Category> categories = new List<Category>();
            _categoriesRepositoryMock.Setup(temp => temp.GetAllCategories()).ReturnsAsync(categories);

            // Act
            List<CategoryResponse> actualResult = await _categoriesGetterService.GetAllCategories();
            // Assert
            actualResult.Should().BeEmpty();
        }

        #region GetAllCategories
        [Fact]
        public async Task GetAllCategories_FewCategories_ToBeSuccessful()
        {
            // Arrange
            List<CategoryAddRequest> categoryAddRequestList = _fixture.CreateMany<CategoryAddRequest>(3).ToList();
            List<Category> categories = categoryAddRequestList.Select(temp => temp.ToCategory()).ToList();
            List<CategoryResponse> categoryResponseList = _mapper.Map<List<CategoryResponse>>(categoryAddRequestList);

            _categoriesRepositoryMock.Setup(temp => temp.GetAllCategories()).ReturnsAsync(categories);

            // Act
            List<CategoryResponse> actualResult = await _categoriesGetterService.GetAllCategories();
            // Assert
            actualResult.Should().BeEquivalentTo(categoryResponseList);
        }
        #endregion
        #region GetCategoryByCategoryID
        [Fact]
        public async Task GetCategoryByCategoryID_NullCategoryID_ToBeSuccessful()
        {
            // Arrange
            Guid? countryID = null;
            // Act
            CategoryResponse? actualCategoryResponse = await _categoriesGetterService.GetCategoryByCategoryID(countryID);
            // Assert
            actualCategoryResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetCategoryByCategoryID_NullCategory_ToBeSuccessful()
        {
            Guid validCategoryID = Guid.NewGuid();
            _categoriesRepositoryMock.Setup(temp => temp.GetCategoryByCategoryID(It.IsAny<Guid>()))
                .ReturnsAsync((Category?)null);
            var result = await _categoriesGetterService.GetCategoryByCategoryID(validCategoryID);
            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCategoryByCategoryID_ValidCategoryID_ToBeSuccessful()
        {
            // Arrange
            CategoryAddRequest categoryAddRequest = _fixture.Create<CategoryAddRequest>();
            Category category = categoryAddRequest.ToCategory();
            CategoryResponse categoryResponse = _mapper.Map<CategoryResponse>(category);
            _categoriesRepositoryMock.Setup(temp => temp.GetCategoryByCategoryID(It.IsAny<Guid>()))
                .ReturnsAsync(category);
            // Act
            CategoryResponse? actualCategoryResponse = await _categoriesGetterService.GetCategoryByCategoryID(categoryResponse.CategoryID);
            // Assert
            actualCategoryResponse.Should().BeEquivalentTo(categoryResponse);
            actualCategoryResponse.Should().Be(categoryResponse);
        }
        #endregion

    }
}
