using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.ProductDTO;
using System.Linq.Expressions;

namespace AllFoods.Core.ServiceContracts.IProductsService
{

    public interface IProductsGetterService
    {

        Task<List<ProductResponse>> GetAllProducts(int pageNumber,int pageSize);

        Task<ProductResponse?> GetProductByProductID(Guid? productID);

        Task<List<ProductResponse>> GetFilteredProduct(string searchBy, string searchString,int pageNumber,int pageSize);

    }
}
