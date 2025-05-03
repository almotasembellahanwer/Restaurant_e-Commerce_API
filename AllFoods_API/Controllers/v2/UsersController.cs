using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.DTO.UserDTO;
using AllFoods.Core.Enums;
using AllFoods.Core.ServiceContracts.IProductsService;
using AllFoods.Core.ServiceContracts.IUsersService;
using AllFoods_API.Filters.ActionFilter;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AllFoods_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersGetterService _usersGetterService;
        private readonly IUsersUpdaterService _usersUpdaterService;
        private readonly IUsersDeleterService _usersDeleterService;
        private readonly IUsersLockerService _usersLockerService;


        private readonly IMapper _mapper;
        private readonly APIResponse _response;

        public UsersController(IUsersGetterService usersGetterService, IUsersUpdaterService usersUpdaterService, IUsersDeleterService usersDeleterService, IUsersLockerService usersLockerService, IMapper mapper)
        {
            _usersGetterService = usersGetterService;
            _usersUpdaterService = usersUpdaterService;
            _usersDeleterService = usersDeleterService;
            _usersLockerService = usersLockerService;
            _response = new();
            _mapper = mapper;
        }
        #region GetAllUsers
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [TypeFilter(typeof(ProductListActionFilter))]
        public async Task<ActionResult<APIResponse>> GetAllUsers()
        {
            try
            {
                List<UserDTO> users = await _usersGetterService.GetAllUsers();
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = users;
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

        #region GetUser
        [HttpGet("get/{userID},", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUser(string? userID)
        {
            try
            {
                if (userID is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "user id should not be null" };
                    return BadRequest(_response);
                }

                UserDTO? userDTO = await _usersGetterService.GetUserDetails(userID);
                if (userDTO is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "user not found" };
                    return NotFound(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = userDTO;

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


        #region UpdateUser
        [HttpPut("update",Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateUser([FromBody] UserUpdatDTO? userUpdateDTO)
        {
            try
            {
                if (userUpdateDTO is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "invalid user" };
                    return BadRequest(_response);
                }
                bool isUserUpdated = await _usersUpdaterService.UpdateUser(userUpdateDTO);
                _response.Result = isUserUpdated;
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

        #region DeleteUser
        [HttpDelete("delete/{email}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteUser(string? email)
        {
            try
            {
                if (email is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid email" };
                    return BadRequest(_response);
                }
                bool isDeleted = await _usersDeleterService.DeleteUserByEmail(email);
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


        #region LockUser
        [HttpPost("lock/{email}", Name = "LockUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> LockUser(string? email)
        {
            try
            {
                if (email is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid email" };
                    return BadRequest(_response);
                }
                bool isLocked = await _usersLockerService.LockUser(email);
                if (isLocked == false)
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
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        #endregion

        #region UnlockUser
        [HttpPost("unlock/{email}", Name = "UnlockUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> UnlockUser(string? email)
        {
            try
            {
                if (email is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid email" };
                    return BadRequest(_response);
                }
                bool isUnlocked = await _usersLockerService.UnlockUser(email);
                if (isUnlocked == false)
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
