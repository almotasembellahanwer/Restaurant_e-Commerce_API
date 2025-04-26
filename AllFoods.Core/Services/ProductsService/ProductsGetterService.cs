using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.ServiceContracts.IProductsService;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Services.ProductsService
{
    public class ProductsGetterService : IProductsGetterService
    {
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;
        private readonly ILogger<ProductsGetterService> _logger;

        public ProductsGetterService(IMapper mapper, IProductsRepository productsRepository, ILogger<ProductsGetterService> logger)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProductResponse>> GetAllProducts()
        {
            List<Product> product = await _productsRepository.GetAllProducts();
            return _mapper.Map<List<ProductResponse>>(product);
        }

        public async Task<List<ProductResponse>> GetFilteredProduct(string searchBy, string? searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return _mapper.Map<List<ProductResponse>>(await _productsRepository.GetAllProducts());

            List<Product> products;
            switch (searchBy)
            {
                case nameof(ProductResponse.ProductName):
                    products = await _productsRepository.GetFilteredProduct(p => p.ProductName.ToLower().Contains(searchString!.Trim().ToLower()));
                    break;
                case nameof(ProductResponse.Description):
                    products = await _productsRepository.GetFilteredProduct(p => p.Description.ToLower().Contains(searchString!.Trim().ToLower()));
                    break;
                case nameof(ProductResponse.Price):
                    products = await _productsRepository.GetFilteredProduct(p => p.Price.ToString().Contains(searchString!.Trim().ToLower()));
                    break;
                case nameof(ProductResponse.CategoryID):
                    products = await _productsRepository.GetFilteredProduct(p => p.CategoryID.ToString()!.Contains(searchString!));
                    break;
                default:
                    products = await _productsRepository.GetAllProducts();
                    break;
            }
            _logger.LogDebug($"SearchBy: {searchBy}, SearchString: {searchString}");
            List<ProductResponse> result = _mapper.Map<List<ProductResponse>>(products);
            return result;
        }

        public async Task<ProductResponse?> GetProductByProductID(Guid? productID)
        {
            if(productID is null)
            {
                return null;
            }

            Product? product = await _productsRepository.GetProductByProductID(productID);
            if(product is null)
            {
                return null;
            }
            return _mapper.Map<ProductResponse>(product);
        }
    }
}
