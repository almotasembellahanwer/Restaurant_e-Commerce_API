using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.ServiceContracts.IUsersService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using AutoMapper;
using AllFoods.Core.Domain.Entities;
using AllFoods.Core.DTO.AccountDTO;
using AllFoods.Core.Exceptions;
using AllFoods.Core.Settinges.NewFolder;
using Microsoft.Extensions.Logging;
using System.Linq;
//using Microsoft.IdentityModel.JsonWebTokens;
namespace AllFoods.Core.Services.UsersService
{
    public class AccountService : IAccountService
    {

        private readonly IAccountRepository _usersRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountService> _logger;

        private readonly IMapper _mapper;
        private string secretKey;

        public AccountService(IAccountRepository usersRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration, IMapper mapper, RoleManager<IdentityRole> roleManager, ILogger<AccountService> logger)
        {
            _usersRepository = usersRepository;
            _userManager = userManager;
            secretKey = configuration.GetSection("APISettings:Secret").ToString()!;
            _mapper = mapper;
            _roleManager = roleManager;
            _logger = logger;
        }



        public async Task<bool> IsUniqueUser(string userName)
        {
            return await _usersRepository.IsUniqueUser(userName);
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            // retrieve the user from repository based on email
            var user = await _usersRepository.GetUser(u=>u.Email!.ToLower() == loginRequestDTO.Email.ToLower());


            // chech if user is null or isValid = false
            if(user is null)
            {
                return new TokenDTO() {
                    AccessToken = "",
                    RefreshToken = ""
                };

            }

            // Check if user is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                return new TokenDTO()
                {
                    AccessToken = "",
                    RefreshToken = "",
                    IsLockedOut = true
                };
            }

            // check if the user is valid or not based on user
            // that is retrieved from database and Password that user entered
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!isValid)
            {
                // Increment failed access count
                await _userManager.AccessFailedAsync(user);
                return new TokenDTO()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
            }
            // Reset failed token on successful login
            await _userManager.ResetAccessFailedCountAsync(user);

            // if user was found generate JWT token
            var jwtTokenID = $"JIT{Guid.NewGuid()}";

            var accessToken = await GetAccessToken(user, jwtTokenID);
            // Create new refreshToken here
            var refreshToken = await _usersRepository.CreateNewRefreshToken(user.Id, jwtTokenID);
            // Store information in LoginResponseDTO and return it
            var tokenDTO = new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokenDTO;
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            // Find the existing refresh token from database
            var existingRefreshToken = await _usersRepository.GetRefreshToken(temp=>temp.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
                return new TokenDTO();
            var user = await _usersRepository.GetUser(temp => temp.Id == existingRefreshToken.UserID);
            // Check if user is locked out
            if (user is not null && await _userManager.IsLockedOutAsync(user))
            {
                return new TokenDTO() { IsLockedOut = true };
            }
            // Compare data from existing refresh token that comes from database
            // and access token provided from tokenDTO and check if has any mismatch
            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserID,existingRefreshToken.JwtTokenID);
            if(isTokenValid == false)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }

            // When someone tries to use not valid refresh token make some logic to that
            if (!existingRefreshToken.IsValid)
            {
                await _usersRepository.MarkAllTokenInChainAsInvalid(existingRefreshToken.UserID, existingRefreshToken.JwtTokenID);
                return new TokenDTO();
            }
            

            // If just expired mark as invalid and return empty

            if(existingRefreshToken.ExpiredAt < DateTime.UtcNow)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }
            // replace the old refresh token with new that has been updated with new expired date
            var newRefreshToken = await _usersRepository.CreateNewRefreshToken(existingRefreshToken.UserID,existingRefreshToken.JwtTokenID);
            // revoke the existing refresh token
            await MarkTokenAsInvalid(existingRefreshToken);

            // generate new access token
            ApplicationUser? existingUser = await _usersRepository.GetUser(temp=>temp.Id == existingRefreshToken.UserID);
            if(existingUser == null)

                return new TokenDTO();

            var newAccessToken = await GetAccessToken(existingUser, existingRefreshToken.JwtTokenID);

            TokenDTO result = new()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            return result;
        }

        public async Task<AccountDTO> Register(RegistrationRequestDTO registerRequestDTO)
        {
            var response = new AccountDTO();

            var user = new ApplicationUser()
            {
                UserName = registerRequestDTO.Email,
                Name = registerRequestDTO.Name,
                Email = registerRequestDTO.Email,
                NormalizedEmail = registerRequestDTO.Email.ToUpper(),
                StreetAddress= registerRequestDTO.StreetAddress,
                City = registerRequestDTO.City,
                State = registerRequestDTO.State,
                PostalCode = registerRequestDTO.PostalCode

            };

            // Use try-catch block to handle registers
            try
            {
                if(registerRequestDTO.Role != RoleSettings.Customer_Role)
                {
                    throw new BadRequestException("Invaid role");
                }
                // Create a user by using userManager
                var userCreated = await _userManager.CreateAsync(user, registerRequestDTO.Password);
                if (userCreated.Succeeded)
                {
                    // Create a customer role if it doesn't exist
                    if (!await _roleManager.RoleExistsAsync(RoleSettings.Customer_Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(RoleSettings.Customer_Role));
                        await _roleManager.CreateAsync(new IdentityRole(RoleSettings.Admin_Role));
                    }
                    

                        await _userManager.AddToRoleAsync(user, RoleSettings.Customer_Role);
                    

                    var userToReturn = await _usersRepository.GetUser(u=>u.Email == registerRequestDTO.Email);
                    return _mapper.Map<AccountDTO>(userToReturn);
                }
                else
                {
                    response.ErrorMassages = userCreated.Errors
                        .Select(temp => temp.Description).ToList();
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("error occured {exception}", ex);
                response.ErrorMassages = new List<string>() { ex.Message };
                return response;
            }
        }
        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            // Get existing refreshToken from database
            var existingRefreshToken = await _usersRepository.GetRefreshToken(temp=>temp.Refresh_Token == tokenDTO.RefreshToken);
            if(existingRefreshToken == null)
                return;

            // Compare data from existing refresh token that comes from database
            // and access token provided from tokenDTO and check if has any mismatch
            var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserID, existingRefreshToken.JwtTokenID);
            if (isTokenValid == false)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return;
            }
            await _usersRepository.MarkAllTokenInChainAsInvalid(existingRefreshToken.UserID, existingRefreshToken.JwtTokenID);

        }
        public async Task CleanUpRefreshTokens(string? email)
        {
            if (email is null)
                throw new BadRequestException("write a valid email");
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                throw new NotFoundException("user not found");
            var oldrefreshTokens = await _usersRepository.GetAllRefreshTokens(user.Id);
            if (oldrefreshTokens.Any())
            {
                await _usersRepository.RemoveRenge(user.Id);
                await _usersRepository.Save();
            }

        }

        #region Helper Methods
        /// <summary>
        /// Get JWT token 
        /// </summary>
        /// <param name="user">user to use while creating JWT token</param>
        /// <param name="jwtTokenID">JWT token id to add to claims</param>
        /// <returns>Returns JWT token</returns>
        private async Task<string> GetAccessToken(ApplicationUser user, string jwtTokenID)
        {
            // get the role that is assigned to user and add it to claim
            var roles = await _userManager.GetRolesAsync(user);
            // Create JWTSecuretyToken instance
            var tokenHandler = new JwtSecurityTokenHandler();
            // Create key based on secretKey that in configured from appSettings.json
            byte[] key = Encoding.ASCII.GetBytes(secretKey);
            // Add SecuretyTokenDescripter information like Subject(calims), Expires, signInCredentials
            var tokenDescripter = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                 // we need to add information about user in claim like userName and role
                    new Claim(ClaimTypes.Name,user.UserName!),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault()!),
                    new Claim(JwtRegisteredClaimNames.Jti,jwtTokenID), // store jwt token id
                    new Claim(JwtRegisteredClaimNames.Sub,user.Id) // store user id

                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                //Issuer = "https://allfood-api.com",
                //Audience = "https://test-allfood-api.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };


            // Create a token
            var token = tokenHandler.CreateToken(tokenDescripter);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }

        /// <summary>
        /// Get JWT token data like jwtTokenID and userID and if it successful or not
        /// </summary>
        /// <param name="accessToken">Access token to read data from it</param>
        /// <returns>Returns userID and jwtTokenID and if successful or not</returns>
        private bool GetAccessTokenData(string accessToken, string expectedUserID, string expectedjwtTokenID)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                // Read the token claims and values
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                // Get jwt token id value from claims 
                var jwtTokenID = jwt.Claims.FirstOrDefault(temp => temp.Type == JwtRegisteredClaimNames.Jti)!.Value;
                // Get user id value from claims 
                var userID = jwt.Claims.FirstOrDefault(temp => temp.Type == JwtRegisteredClaimNames.Sub)!.Value;

                return expectedUserID == userID && expectedjwtTokenID == jwtTokenID;
            }
            catch
            {
                return false;
            }
        }



        private async Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            await _usersRepository.Save();
        }
        #endregion

    }
}
