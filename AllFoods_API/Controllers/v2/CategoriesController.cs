using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.ServiceContracts.ICategoriesService;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace AllFoods_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesGetterService _categoriesGetterService;
        private readonly ICategoriesAdderService _categoriesAdderService;
        private readonly ICategoriesUpdaterService _categoriesUpdaterService;
        private readonly ICategoriesDeleterService _categoriesDeleterService;

        private readonly APIResponse _response;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoriesGetterService categoriesGetterService, IMapper mapper, ICategoriesAdderService categoriesAdderService, ICategoriesUpdaterService categoriesUpdaterService, ICategoriesDeleterService categoriesDeleterService)
        {
            _categoriesGetterService = categoriesGetterService;
            _response = new APIResponse();
            _mapper = mapper;
            _categoriesAdderService = categoriesAdderService;
            _categoriesUpdaterService = categoriesUpdaterService;
            _categoriesDeleterService = categoriesDeleterService;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<APIResponse>> GetAllCategories()
        {
            try
            {
                // Get All categories from service
                List<CategoryResponse> categories = await _categoriesGetterService.GetAllCategories();
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = categories;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return _response;

        }

        [HttpGet("GetCategory/{categoryID:Guid},", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetCategory(Guid? categoryID)
        {
            try
            {
                if (categoryID is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "category id should not be null" };
                    return BadRequest(_response);
                }

                CategoryResponse? categoryResponse = await _categoriesGetterService.GetCategoryByCategoryID(categoryID);
                if (categoryResponse is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "category not found" };
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = categoryResponse;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;

        }


        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CategoryAddRequest? categoryAddRequest)
        {
            try
            {
                if (categoryAddRequest is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "category that you want to add should not be null" };
                    return BadRequest(_response);
                }

                CategoryResponse? categoryResponse = await _categoriesAdderService.AddCategory(categoryAddRequest);
                if (categoryResponse.CategoryName is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "category name not found" };
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = categoryResponse;

                return CreatedAtRoute("GetCategory", new { categoryID = categoryResponse.CategoryID }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;

        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateCategory([FromBody] CategoryUpdateRequest? categoryUpdateRequest)
        {
            try
            {
                if (categoryUpdateRequest is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "category that you want to update should not be null" };
                    return BadRequest(_response);
                }

                CategoryResponse? categoryResponse = await _categoriesUpdaterService.UpdateCategory(categoryUpdateRequest);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = categoryResponse;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;

        }


        [HttpDelete("Delete/{categoyID:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteCategory(Guid? categoyID)
        {
            try
            {
                if (categoyID is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "category id not found" };
                    return BadRequest(_response);
                }

               await _categoriesDeleterService.DeleteCategory(categoyID);

                _response.StatusCode = HttpStatusCode.NoContent;

                return NoContent(); ;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;

        }
    }
}
