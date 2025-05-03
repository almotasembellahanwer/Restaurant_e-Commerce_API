using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AllFoods.Core.Settinges
{
    public static class CacheSettings
    {
        public static string ProductsKey(ProductQueryParameters parameters)
        {
            List<string> normalizedParams = new List<string>()
            {
                $"searchBy:{parameters.SearchBy}",
                $"SearchString:{parameters.SearchString ?? null}",
                $"SortBy:{parameters.SortBy}",
                $"SortOrder:{parameters.SortOrder}",
                $"PageNumber:{parameters.PageNumber}",
                $"PageSize:{parameters.PageSize}",

            };
            return $"products_{JsonSerializer.Serialize(normalizedParams).GetHashCode()}";
        }

        public static string ProductKey(Guid? productID)
        {
            return $"product_{productID}";
        }
        public static string ProductsPattern()
        {
            return $"products_*";
        }
    }
}
