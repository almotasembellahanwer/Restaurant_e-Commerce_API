using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using AllFoods.Core.Settinges;
using AllFoods.Core.ServiceContracts.IProductsService;
using AllFoods.Core.DTO.ProductDTO;
namespace AllFoods.Core.Services.ProductsService
{
    public class ProductsAdderService : IProductsAdderService
    {
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;
        //private readonly IHostEnvironment _webHostEnvironment;
        private readonly string _imagePath;

        public ProductsAdderService(IMapper mapper, IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _imagePath = $"wwwroot{FileSettings.ImagesPath}";
        }
        #region AddProduct
        public async Task<ProductResponse> AddProduct(ProductAddRequest? productAddRequest)
        {
            // check if productAddRequest is null
            if (productAddRequest == null)
            {
                throw new ArgumentNullException($"{nameof(productAddRequest)} should not be null");
            }
            if(productAddRequest.ProductName is null)
            {
                throw new ArgumentException("product name should not be null");
            }
            // create image
            if(productAddRequest.ImageUrl != null)
            {
                productAddRequest.ImageUrl = await SaveCoverHelper.SaveCover(productAddRequest.Cover, "products");
            }
            else
            {
                productAddRequest.ImageUrl = "https://placehold.co/600x400";
            }

            // Validate all propertoies of the productAddRequest
            ValidationHelper.ModelValidate(productAddRequest);
            // Convert the productAddRequest from productAddRequest object to a Product object
            Product? product = _mapper.Map<Product>(productAddRequest);
            // Generate a new ProductID
            //product.ProductID = Guid.NewGuid();
            // Add the person to the list of persons
            await _productsRepository.AddProduct(product);
            // return a ProductResponse object

            return _mapper.Map<ProductResponse>(product);
        }

    
        #endregion

    }
}
