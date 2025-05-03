using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Enums;
using AllFoods.Core.MappingCofig;
using AllFoods.Core.ServiceContracts;
using AllFoods.Core.Services.ProductsService;
using AutoFixture;
using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace AllFoods.ServiceTests
{
    public class ProductsServiceTest
    {
        private readonly ProductsAdderService _productsAdderService;
        private readonly ProductsUpdaterService _productsUpdaterService;
        private readonly ProductsDeleterService _productsDeleterService;
        private readonly ProductsGetterService _productsGetterService;
        private readonly ProductsSorterService _productsSorterService;

        private readonly IFixture _fixture;
        private readonly IMapper _mapper;

        private readonly Mock<IProductsRepository> _productsRepositoryMock;
        private readonly IProductsRepository _productsRepository;
        public ProductsServiceTest()
        {
            _productsRepositoryMock = new Mock<IProductsRepository>();
            _productsRepository = _productsRepositoryMock!.Object;

            _fixture = new Fixture();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfig>();
            });
            _mapper = config.CreateMapper();
            var loggerMock = new Mock<ILogger<ProductsGetterService>>();
            _productsAdderService = new ProductsAdderService(_mapper, _productsRepository!);
            _productsGetterService = new ProductsGetterService(_mapper, _productsRepository!, loggerMock.Object);
            _productsUpdaterService = new ProductsUpdaterService(_mapper, _productsRepository!);
            _productsDeleterService = new ProductsDeleterService(_mapper, _productsRepository!);
            _productsSorterService = new ProductsSorterService();
        }

        #region AddProduct


        // When ProductAddRequest is null, it should throw ArgumentException
        [Fact]
        public async Task AddProduct_NullProduct_ToBeArgumentNullException()
        {
            // Arrange
            ProductAddRequest? productAddRequest = null;
            Func<Task> action = async () =>
            {
                // Act
                await _productsAdderService.AddProduct(productAddRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        // When ProductName is null, it should throw ArgumentException
        [Fact]
        public async Task AddProduct_ProductNameIsNull_ToBeArgumentException()
        {
            // Arrange
            ProductAddRequest? ProductNameAddRequest = _fixture.Build<ProductAddRequest>()
                .With(temp => temp.ProductName, null as string)
                .Create();
            Product product = _mapper.Map<Product>(ProductNameAddRequest);
            // When personRepository.AddPerson is called,
            // it has to return the same Person object
            _productsRepositoryMock.Setup(temp => temp.AddProduct(It.IsAny<Product>())).ReturnsAsync(product);
            Func<Task> action = async () =>
                await _productsAdderService.AddProduct(ProductNameAddRequest);
            // Act and Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }



        //When you supply proper category name, it should insert (add) the category to the existing list of categories
        [Fact]
        public async Task AddProduct_FullProduct_ToBeSuccessful()
        {
            // Arrange
            ProductAddRequest productAddRequest = _fixture.Build<ProductAddRequest>()
                .Create();

            // Convert to Product
            Product product = _mapper.Map<Product>(productAddRequest);

            _productsRepositoryMock.Setup(temp => temp.AddProduct(It.IsAny<Product>())).ReturnsAsync(product);

            // Act
            ProductResponse productResponseFromAdd = await _productsAdderService.AddProduct(productAddRequest);
            product.CategoryID = productResponseFromAdd.CategoryID;

            // Assert
            productResponseFromAdd.Should().NotBeNull();
            productResponseFromAdd.CategoryID.Should().Be(product.CategoryID);

        }
        #endregion


        #region GetAllProducts
        [Fact]

        public async Task GetAllProducts_EmptyList_ToBeSuccessful()
        {
            // Arrange
            List<Product> products = new List<Product>();
            _productsRepositoryMock.Setup(temp => temp.GetAllProducts()).ReturnsAsync(products);

            // Act
            List<ProductResponse> actualResult = await _productsGetterService.GetAllProducts();
            // Assert
            actualResult.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllProducts_FewProducts_ToBeSuccessful()
        {
            // Arrange
            List<ProductAddRequest> productAddRequestList = _fixture.CreateMany<ProductAddRequest>(3).ToList();
            List<Product> products = _mapper.Map<List<Product>>(productAddRequestList);
            List<ProductResponse> categoryResponseList = _mapper.Map<List<ProductResponse>>(productAddRequestList);

            _productsRepositoryMock.Setup(temp => temp.GetAllProducts()).ReturnsAsync(products);

            // Act
            List<ProductResponse> actualProductFromGet = await _productsGetterService.GetAllProducts();
            // Assert
            actualProductFromGet.Should().BeEquivalentTo(categoryResponseList);
        }
        #endregion
        #region GetProductByProductID
        [Fact]
        public async Task GetProductByProductID_NullProductID_ToBeSuccessful()
        {
            // Arrange
            Guid? productID = null;
            // Act
            ProductResponse? actualProductFromGet = await _productsGetterService.GetProductByProductID(productID);
            // Assert
            actualProductFromGet.Should().BeNull();
        }

        [Fact]
        public async Task GetProductByProductID_NullProduct_ToBeSuccessful()
        {
            Guid validProductID = Guid.NewGuid();
            _productsRepositoryMock.Setup(temp => temp.GetProductByProductID(It.IsAny<Guid>()))
                .ReturnsAsync((Product?)null);
            var result = await _productsGetterService.GetProductByProductID(validProductID);
            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetProductByProductID_ValidProductID_ToBeSuccessful()
        {
            // Arrange
            ProductAddRequest productAddRequest = _fixture.Create<ProductAddRequest>();
            Product product = _mapper.Map<Product>(productAddRequest);
            ProductResponse productResponse = _mapper.Map<ProductResponse>(product);
            _productsRepositoryMock.Setup(temp => temp.GetProductByProductID(It.IsAny<Guid>()))
                .ReturnsAsync(product);
            // Act
            ProductResponse? actualCategoryResponse = await _productsGetterService.GetProductByProductID(productResponse.CategoryID);
            // Assert
            actualCategoryResponse.Should().BeEquivalentTo(productResponse);
            actualCategoryResponse.Should().Be(productResponse);
        }
        #endregion

        #region GetFilteredProduct
        [Fact]
        public async Task GetFilteredProduct_EmptySearchedText_ToBeSuccessful()
        {
            // Arrange
            List<Product> product = new List<Product>() {
                _fixture.Build<Product>()
                .With(temp=>temp.Category,null as Category)
                .Create(),
                _fixture.Build<Product>()
                .With(temp=>temp.Category,null as Category)
                .Create(),
                _fixture.Build<Product>()
                .With(temp=>temp.Category,null as Category)
                .Create(),
            };
            List<ProductResponse> expectedProduct = _mapper.Map<List<ProductResponse>>(product);

            _productsRepositoryMock.Setup(temp => temp.GetFilteredProduct(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);
            // Act
            List<ProductResponse> actualProductFromGet = await _productsGetterService.GetFilteredProduct(nameof(ProductResponse.ProductName), "");
            // Assert
            actualProductFromGet.Should().BeEquivalentTo(expectedProduct);
        }


        [Fact]
        public async Task GetFilteredProduct_SearchByProductName_ToBeSuccessful()
        {
            // Arrange
            List<Product> product = new List<Product>() {
                _fixture.Build<Product>()
                .With(temp=>temp.ProductName,"mohammed")
                .With(temp=>temp.Category,null as Category)
                .Create(),
                _fixture.Build<Product>()
                .With(temp=>temp.ProductName,"Ahmed")
                .With(temp=>temp.Category,null as Category)
                .Create(),
                _fixture.Build<Product>()
                .With(temp=>temp.ProductName,"mohammed")
                .With(temp=>temp.Category,null as Category)
                .Create(),
            };
            List<ProductResponse> expectedProduct = _mapper.Map<List<ProductResponse>>(product);

            _productsRepositoryMock.Setup(temp => temp.GetFilteredProduct(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);
            // Act
            List<ProductResponse> actualProductFromGet = await _productsGetterService.GetFilteredProduct(nameof(ProductResponse.ProductName), "ed");
            // Assert
            actualProductFromGet.Should().OnlyContain(temp=>temp.ProductName.Contains("ed",StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region UpdateProduct
        [Fact]
        public async Task UpdateProduct_NullProduct_ToBeArgumentNullException()
        {
            // Arrange
            ProductUpdateRequest? productUpdateRequest = null;
            Func<Task> action = async () =>
            {
                // Act
                await _productsUpdaterService.UpdateProduct(productUpdateRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateProduct_InvalidProductID_ToBeArgumentException()
        {
            // Arrange
            ProductUpdateRequest productUpdateRequest = _fixture.Create<ProductUpdateRequest>();
            // Act
            Func<Task> action = async () =>
            {
                await _productsUpdaterService.UpdateProduct(productUpdateRequest);
            };
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task UpdateProduct_NullProductName_ToBeArgumentException()
        {
            // Arrange
            ProductUpdateRequest productUpdateRequest = _fixture.Build<ProductUpdateRequest>()
                .With(temp=>temp.ProductName,null as string)
                .Create();
            // Act
            Func<Task> action = async () =>
            {
                await _productsUpdaterService.UpdateProduct(productUpdateRequest);
            };
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

        }


        [Fact]
        public async Task UpdateProduct_FullProductDetails_ToBeSuccessful()
        {
            // Arrange
            ProductUpdateRequest productUpdateRequest = _fixture.Build<ProductUpdateRequest>()
                .Create();

            // Convert to Product
            Product product = _mapper.Map<Product>(productUpdateRequest);
            // Convert to ProductResponse

            ProductResponse expectedproductResponse = _mapper.Map<ProductResponse>(product);
            _productsRepositoryMock.Setup(temp => temp.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(product);
            _productsRepositoryMock.Setup(temp => temp.GetProductByProductID(It.IsAny<Guid>())).ReturnsAsync(product);

            // Act
            ProductResponse? productResponseFromUpdate = await _productsUpdaterService.UpdateProduct(productUpdateRequest);

            // Assert
            productResponseFromUpdate.Should().Be(expectedproductResponse);

        }
        #endregion

        #region DeleteProduct
        [Fact]
        public async Task DeleteProduct_ValidProductID()
        {
            //Arrange
            Product product = _fixture.Build<Product>()
             .With(temp => temp.Category, null as Category)
             .Create();


            _productsRepositoryMock
             .Setup(temp => temp.DeleteProductByProductID(It.IsAny<Guid>()))
             .ReturnsAsync(true);

            _productsRepositoryMock
             .Setup(temp => temp.GetProductByProductID(It.IsAny<Guid>()))
             .ReturnsAsync(product);

            //Act
            bool isDeleted = await _productsDeleterService.DeleteProduct(product.ProductID);

            //Assert
            isDeleted.Should().BeTrue();
        }
        [Fact]
        public async Task DeleteProduct_InvalidProductID()
        {
            //Act
            bool isDeleted = await _productsDeleterService.DeleteProduct(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();
        }
        #endregion


        #region GetSortedProducts
        //When we sort based on PersonName in DESC,
        //it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedProducts_ToBeSuccessful()
        {
            //Arrenge
            List<Product> products = new List<Product>()
            {
                _fixture.Build<Product>()
                .With(temp=>temp.Category,null as Category).Create(),

            _fixture.Build<Product>()
                .With(temp => temp.Category, null as Category).Create(),

             _fixture.Build<Product>()
                .With(temp => temp.Category, null as Category).Create()
            };


            List<ProductResponse> expectedProductList = _mapper.Map<List<ProductResponse>>(products);

            _productsRepositoryMock.Setup(temp => temp.GetAllProducts())
                .ReturnsAsync(products);

            List<ProductResponse> allPersons = await _productsGetterService.GetAllProducts();
            List<ProductResponse> actualProductResponseListFromSort = _productsSorterService.GetSortedProducts(allPersons, nameof(ProductResponse.ProductName), SortOrderOptions.DESC);
           
            // Assert
            actualProductResponseListFromSort.Should().BeInDescendingOrder(p => p.ProductName);
        }
        #endregion
    }
}
