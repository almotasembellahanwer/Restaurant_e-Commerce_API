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
            if (context.ActionArguments.TryGetValue("parameters",out var parametersObj)
                && parametersObj is ProductQueryParameters parameters)
            {
                if (parameters.SearchBy is not null)
                {
                    var productsOptions = new List<string> {
                        nameof(ProductResponse.ProductName),
                        nameof(ProductResponse.Price),
                        nameof(ProductResponse.Description),
                        nameof(ProductResponse.ImageUrl),
                        nameof(ProductResponse.CategoryID),
                    };

                    if (productsOptions.Any(temp => temp == parameters.SearchBy) == false)
                    {
                        _logger.LogInformation("searchBy acual value: {searchBy}", parameters.SearchBy);
                        parameters.SearchBy = nameof(ProductResponse.ProductName);
                        _logger.LogInformation("searchBy updated value {searchBy}", parameters.SearchBy);
                    }
                }
            }
        }
    }
}
