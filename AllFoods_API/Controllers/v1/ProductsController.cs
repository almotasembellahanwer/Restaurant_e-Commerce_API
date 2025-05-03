using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Enums;
using AllFoods.Core.ServiceContracts.IProductsService;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace AllFoods_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsAdderService _productsAdderService;
        private readonly IProductsGetterService _productsGetterService;
        private readonly IProductsSorterService _productsSorterService;
        private readonly IProductsUpdaterService _productsUpdaterService;
        private readonly IProductsDeleterService _productsDeleterService;

        private readonly IMapper _mapper;
        private readonly APIResponse _response;

        public ProductsController(IProductsGetterService productsGetterService, IProductsSorterService productsSorterService, IProductsAdderService productsAdderService, IMapper mapper, IProductsDeleterService productsDeleterService, IProductsUpdaterService productsUpdaterService)
        {
            _productsGetterService = productsGetterService;
            _productsSorterService = productsSorterService;
            _response = new();
            _productsAdderService = productsAdderService;
            _mapper = mapper;
            _productsDeleterService = productsDeleterService;
            _productsUpdaterService = productsUpdaterService;
        }
        #region GetAllProducts
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllProducts(string searchBy, string? searchString, string sortBy = nameof(ProductResponse.ProductName), SortOrderOptions sortOrder = SortOrderOptions.ASC,int pageNumber = 1,int pageSize = 10)
        {
            try
            {
                List<ProductResponse> products = await _productsGetterService.GetFilteredProduct(searchBy, searchString,pageNumber,pageSize);

                List<ProductResponse> sortedProducts = await _productsSorterService.GetSortedProducts(products, sortBy, sortOrder);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = sortedProducts;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;

        }

        #endregion

        #region GetProduct
        [HttpGet("{productID:Guid},", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProduct(Guid? productID)
        {
            try
            {
                if (productID is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "product id should not be null" };
                    return BadRequest(_response);
                }

                ProductResponse? productResponse = await _productsGetterService.GetProductByProductID(productID);
                if (productResponse is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "product not found" };
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = productResponse;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;

        }
        #endregion

        #region CreateProduct
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateProduct([FromForm] ProductAddRequest? productAddRequest)
        {
            try
            {
                if (productAddRequest is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "product that you want to add should not be null" };
                    return BadRequest(_response);
                }

                ProductResponse? productResponse = await _productsAdderService.AddProduct(productAddRequest);
                if (productResponse.ProductName is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "product name not found" };
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = productResponse;

                return CreatedAtRoute("GetProduct", new { productID = productResponse.ProductID }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        #endregion

        #region UpdateProduct
        [HttpPut(Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateProduct([FromForm] ProductUpdateRequest? productUpdateRequest)
        {
            try
            {
                if (productUpdateRequest is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "invalid product" };
                    return BadRequest(_response);
                }
                ProductResponse? productResponse = await _productsUpdaterService.UpdateProduct(productUpdateRequest);
                _response.Result = productResponse;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }

        #endregion

        #region DeleteProduct
        [HttpDelete("{productID:Guid}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteProduct(Guid? productID)
        {
            try
            {
                if (productID is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid product id" };
                    return BadRequest(_response);
                }
                bool isDeleted = await _productsDeleterService.DeleteProduct(productID);
                if (isDeleted == false)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion

    }
}
