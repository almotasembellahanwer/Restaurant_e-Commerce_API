using AllFoods.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.ProductDTO
{
    public class ProductQueryParameters
    {
        public string SearchBy { get; set; } = string.Empty;
        public string? SearchString { get; set; }
        public string SortBy { get; set; } = nameof(ProductResponse.ProductName);
        public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;
        [Range(1,int.MaxValue,ErrorMessage = "{0} should not be less than 1")]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
