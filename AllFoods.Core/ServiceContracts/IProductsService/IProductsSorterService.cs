using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.IProductsService
{
    public interface IProductsSorterService
    {
        Task<List<ProductResponse>> GetSortedProducts(List<ProductResponse> allProducts, string sortBy, SortOrderOptions sortOrder);
    }
}
