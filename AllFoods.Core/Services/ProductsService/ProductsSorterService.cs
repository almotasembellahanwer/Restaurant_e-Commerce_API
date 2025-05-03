using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Enums;
using AllFoods.Core.ServiceContracts.IProductsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Services.ProductsService
{
    public class ProductsSorterService : IProductsSorterService
    {
        public Task<List<ProductResponse>> GetSortedProducts(List<ProductResponse> allProducts, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return Task.FromResult(allProducts);
            }
            // Get type
            Type productResponseType = typeof(ProductResponse);
            // Get properity of class type
            PropertyInfo? sortByProperty = productResponseType.GetProperty(sortBy);
            if (sortByProperty is null)
            {
                return Task.FromResult(allProducts);
            }
            // Create an empty sorted list
            IOrderedEnumerable<ProductResponse> sortList;
            // Check if sortOrderOptions is ASC or DESC and add the value into sortList

            if (sortOrder == SortOrderOptions.ASC)
            {
                sortList = sortByProperty.GetType() == typeof(string) ? allProducts.OrderBy(product => (string?)sortByProperty.GetValue(product), StringComparer.OrdinalIgnoreCase) : allProducts.OrderBy(product => sortByProperty.GetValue(product)!.ToString());
            }
            else
            {
                sortList = sortByProperty.GetType() == typeof(string) ? allProducts.OrderByDescending(product => (string?)sortByProperty.GetValue(product), StringComparer.OrdinalIgnoreCase) : allProducts.OrderByDescending(product => sortByProperty.GetValue(product)!.ToString());
            }

            return Task.FromResult(sortList.ToList());
        }
    }
}
