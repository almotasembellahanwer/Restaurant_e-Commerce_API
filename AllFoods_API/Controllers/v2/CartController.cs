using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.DTO.CartItemDTO;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.Enums;
using AllFoods.Core.ServiceContracts.ICartsService;
using AllFoods.Core.ServiceContracts.IProductsService;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
namespace AllFoods_API.Controllers.v2

{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartsAdderService _cartsAdderService;
        private readonly ICartsGetterService _cartsGetterService;
        private readonly ICartsDeleterService _cartsDeleterService;
        private readonly IMapper _mapper;

        private readonly ILogger<CartController> _logger;


        private readonly APIResponse _response;

        public CartController(ICartsAdderService cartsAdderService, ICartsGetterService cartsGetterService, ILogger<CartController> logger, IMapper mapper, ICartsDeleterService cartsDeleterService)
        {
            _response = new();
            _cartsAdderService = cartsAdderService;
            _cartsGetterService = cartsGetterService;
            _logger = logger;
            _mapper = mapper;
            _cartsDeleterService = cartsDeleterService;
        }

        [HttpGet("GetAllProductFromCart")]
        public async Task<ActionResult<APIResponse>> GetCart()
        {
            var userID = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID is null)
            {
                return Unauthorized("User not authenticated.");
            }
            try
            {
                CartDTO cart = await _cartsGetterService.GetCart(userID);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = cart;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
        [HttpPost("AddToCart")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateCartItem([FromBody] CartItemRequestDTO cartItemRequestDTO)
        {
            
            try
            {
                var userID = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"UserID : {ClaimTypes.NameIdentifier},{userID}, {User}");
                _logger.LogInformation($"cartItemRequestDTO : {cartItemRequestDTO}");

                if (userID is null)
                {
                    return Unauthorized("User not authenticated.");
                }
                if (cartItemRequestDTO is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add($"Cart item should not be null");
                    return BadRequest(_response);
                }


                await _cartsAdderService.AddToCart(cartItemRequestDTO, userID);
                _response.StatusCode = HttpStatusCode.Created;
                return Created();
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("Remove/{productID:Guid}")]
        public async Task<ActionResult<APIResponse>> RemoveFromCart(Guid? productID)
        {
            try
            {
                var userID = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userID is null)
                    return Unauthorized("user not ahthorized");

                if (productID is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add($"Invalid productID");
                    return BadRequest(_response);
                }
                await _cartsDeleterService.DeleteFromCart(productID, userID);
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
        [HttpPost("Increment")]
        public async Task<ActionResult<APIResponse>> Increment([FromBody] QuantityDTO quantityDTO)
        {
            try
            {
                var userID = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userID is null)
                    return Unauthorized("user not ahthorized");

                if (quantityDTO.ProductID is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add($"Invalid productID");
                    return BadRequest(_response);
                }
                QuantityDTO quantity =  await _cartsAdderService.IncrementItemQuantity(quantityDTO.ProductID, userID);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok("Incremented successfully");
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPost("Decrement")]
        public async Task<ActionResult<APIResponse>> Decrement([FromBody] QuantityDTO quantityDTO)
        {
            try
            {
                var userID = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userID is null)
                    return Unauthorized("user not ahthorized");

                if (quantityDTO.ProductID is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add($"Invalid productID");
                    return BadRequest(_response);
                }
                QuantityDTO quantity = await _cartsDeleterService.DecrementItemQuantity(quantityDTO.ProductID, userID);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok("Decremented successfully");
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}

