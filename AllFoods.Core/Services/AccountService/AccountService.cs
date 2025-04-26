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
//using Microsoft.IdentityModel.JsonWebTokens;
namespace AllFoods.Core.Services.UsersService
{
    public class AccountService : IAccountService
    {

        private readonly IAccountRepository _usersRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IMapper _mapper;
        private string secretKey;

        public AccountService(IAccountRepository usersRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _usersRepository = usersRepository;
            _userManager = userManager;
            secretKey = configuration.GetSection("APISettings:Secret").ToString()!;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            return await _usersRepository.IsUniqueUser(userName);
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            // retrieve the user from repository based on userName
            var user = await _usersRepository.GetUser(u=>u.UserName!.ToLower() == loginRequestDTO.UserName.ToLower());
            // check if the user is valid or not based on user
            // that is retrieved from database and Password that user entered
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            // chech if user is null or isValid = false
            if(user is null || isValid == false)
            {
                return new TokenDTO() {
                    AccessToken = "",
                    RefreshToken = ""
                };

            }


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
            // Convert RegistrationRequestDTO to
            // ApplicationUser and store all information to database
            var user = new ApplicationUser()
            {
                UserName = registerRequestDTO.UserName,
                Name = registerRequestDTO.Name,
                Email = registerRequestDTO.UserName,
                NormalizedEmail = registerRequestDTO.UserName.ToUpper(),
                StreetAddress= registerRequestDTO.StreetAddress,
                City = registerRequestDTO.City,
                State = registerRequestDTO.State,
                PostalCode = registerRequestDTO.PostalCode

            };

            // Use try-catch block to handle registers
            try
            {
                // Create a user by using userManager
                var userCreated = await _userManager.CreateAsync(user, registerRequestDTO.Password);
                // If creation succeded add a role to a user that is in userManager
                if (userCreated.Succeeded)
                {
                    // If role exist in role table in database
                    if(!await _roleManager.RoleExistsAsync(registerRequestDTO.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerRequestDTO.Role));
                    }
                    // Assign the user to the specified role and add userId, roleId to AspNetUserRoles table
                    await _userManager.AddToRoleAsync(user, registerRequestDTO.Role);
                    // retrieve user from database
                    var userToReturn = await _usersRepository.GetUser(u=>u.UserName == registerRequestDTO.UserName);
                    //  Map ApplicationUser to UserDTO and return it
                    return _mapper.Map<AccountDTO>(userToReturn);
                }
            }
            catch (Exception ex)
            {
                
            }
            // If something not valid
            return new AccountDTO();
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
                var jwtTokenID = jwt.Claims.FirstOrDefault(temp=>temp.Type == JwtRegisteredClaimNames.Jti)!.Value;
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
    }
}
