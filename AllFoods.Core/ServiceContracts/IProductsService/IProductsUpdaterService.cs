using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.ProductDTO;

namespace AllFoods.Core.ServiceContracts.IProductsService
{
    public interface IProductsUpdaterService
    {

        Task<ProductResponse?> UpdateProduct(ProductUpdateRequest? product);

    }
}
