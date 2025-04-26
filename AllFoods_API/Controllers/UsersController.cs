using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.AccountDTO;
using AllFoods.Core.ServiceContracts.IUsersService;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AllFoods_API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        private readonly IAccountService _usersService;
        private readonly APIResponse _response;

        public UsersController(IAccountService usersService)
        {
            _usersService = usersService;
            _response = new();
        }

        [HttpGet("Error")]
        public async Task<IActionResult> Error()
        {
            throw new FileNotFoundException();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                // use Login Service
                TokenDTO tokenDTO = await _usersService.Login(loginRequestDTO);
                // If tokenDTO is null or token is null or empty return bad request
                if (tokenDTO is null || string.IsNullOrEmpty(tokenDTO.AccessToken))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("UserName or password is incorrect");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = tokenDTO;
                return Ok(_response);
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString()};
            }


            return _response;
        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<APIResponse>> Register([FromBody] RegistrationRequestDTO registerRequestDTO)
        {
            try
            {
                // Chech if user is unique or not
                bool userNameExist = await _usersService.IsUniqueUser(registerRequestDTO.UserName);
                if (userNameExist == false)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username already exist");
                    return BadRequest(_response);
                }
                // use Register Service
                AccountDTO user = await _usersService.Register(registerRequestDTO);
                // If user is null return bad request
                if (user is null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while registration");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = user;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }


            return _response;
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<APIResponse>> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TokenDTO? tokenResponse = await _usersService.RefreshAccessToken(tokenDTO);


                    if (tokenResponse is null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        _response.ErrorMessages.Add("Token Invalid");
                        return BadRequest(_response);
                    }

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = tokenResponse;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid Input");
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }


            return _response;
        }


        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<APIResponse>> RevokeRefreshToken([FromBody] TokenDTO tokenDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _usersService.RevokeRefreshToken(tokenDTO);

                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }

                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid Token");
                    return BadRequest(_response);

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
