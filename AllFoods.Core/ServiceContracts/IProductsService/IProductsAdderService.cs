using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.ProductDTO;
using Microsoft.AspNetCore.Http;

namespace AllFoods.Core.ServiceContracts.IProductsService
{

    public interface IProductsAdderService
    {

        Task<ProductResponse> AddProduct(ProductAddRequest? productAddRequest);


    }
}
