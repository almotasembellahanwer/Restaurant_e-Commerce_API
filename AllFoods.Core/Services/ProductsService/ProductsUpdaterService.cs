using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Helpers;
using AllFoods.Core.ServiceContracts.IProductsService;
using AllFoods.Core.Settinges;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Services.ProductsService
{
    public class ProductsUpdaterService : IProductsUpdaterService
    {
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;
        private readonly string _imagePath;

        public ProductsUpdaterService(IMapper mapper, IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _imagePath = $"wwwroot{FileSettings.ImagesPath}";
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest? productToUpdate)
        {
            // If Product that i want to update is null throw ArgumentNullException
            if (productToUpdate is null)
            {
                throw new ArgumentNullException($"{nameof(productToUpdate)} should not be null");
            }
            // validation
            ValidationHelper.ModelValidate(productToUpdate);
            // Get existing product from repository
            Product? matchingProduct = await _productsRepository.GetProductByProductID(productToUpdate.ProductID);
            // Check if product is not null && productID from productToUpdate not equal existing product
            if (matchingProduct is null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            
            }
            bool hasCover = productToUpdate.Cover is not null;
            string oldCover = matchingProduct.ImageUrl;
            matchingProduct.ProductID = productToUpdate.ProductID;
            matchingProduct.ProductName = productToUpdate.ProductName;
            matchingProduct.Description = productToUpdate.Description;
            matchingProduct.Price = productToUpdate.Price;
            matchingProduct.CategoryID = productToUpdate.CategoryID;
            if (hasCover)
            {
                matchingProduct.ImageUrl = await SaveCoverHelper.SaveCover(productToUpdate.Cover!, "products");
                //var cover = Path.Combine(_imagePath,oldCover); // wwwroot\\assets\\images\\ empty
                //File.Delete(cover);
                if(!string.IsNullOrEmpty(oldCover))
                    DeleteCoverHelper.DeleteFile(oldCover,"products");
            }
            await _productsRepository.UpdateProduct(matchingProduct);
            return _mapper.Map<ProductResponse>(matchingProduct);
        }
    }
}
