using AllFoods.Core.DTO.ProductDTO;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AllFoods_API.Filters.ActionFilter
{
    public class ProductListActionFilter : IActionFilter
    {
        private readonly ILogger<ProductListActionFilter> _logger;

        public ProductListActionFilter(ILogger<ProductListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{ProductListActionFilter}.{OnActionExecuted}", nameof(ProductListActionFilter), nameof(OnActionExecuted));
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var productsOptions = new List<string> {
                        nameof(ProductResponse.ProductName),
                        nameof(ProductResponse.Price),
                        nameof(ProductResponse.Description),
                        nameof(ProductResponse.ImageUrl),
                        nameof(ProductResponse.CategoryID),
                    };

                    if (productsOptions.Any(temp => temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy acual value: {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(ProductResponse.ProductName);
                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
