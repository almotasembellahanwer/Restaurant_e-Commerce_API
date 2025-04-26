using AllFoods.Core.Domain.Entities;
using System.Linq.Expressions;

namespace AllFoods.Core.ServiceContracts.IProductsService
{

    public interface IProductsDeleterService
    {

        Task<bool> DeleteProduct(Guid? productID);

    }
}
