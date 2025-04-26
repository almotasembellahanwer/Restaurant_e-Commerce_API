using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.UserDTO;
using AllFoods.Core.ServiceContracts.IUsersService;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllFoods.Core.Services.UsersService
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }



        public async Task<List<UserDTO>> GetAllUsers()
        {
            List<ApplicationUser> users = await _userManager.Users.ToListAsync();
            List<UserDTO> userDTOList = _mapper.Map<List<UserDTO>>(users);
            foreach(var userDTO in userDTOList)
            {
                var user = users.FirstOrDefault(temp => temp.Id == userDTO.Id);
                if(user != null)
                {
                    string? role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    if(role != null)
                        userDTO.Role = role;
                }

            }
            return userDTOList;
        }

        public async Task<UserDTO?> GetUserDetails(string? userID)
        {
            if (userID is null)
                throw new ArgumentException("userID can not be null");
           ApplicationUser? user = await _userManager.FindByIdAsync(userID);
            if (user is null)
                return null;

            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        public async Task<bool> LockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return false;
             await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return true;
        }

        public async Task<bool> UnlockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            return true;
        }

        public async Task<bool> UpdateUser(string userID, UserDTO userDTO)
        {
            if (string.IsNullOrEmpty(userID) || userDTO == null)
            {
                return false;
            }
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null)
            {
                return false;
            }
            user.Name = userDTO.Name;
            user.Email = userDTO.Email;
            user.StreetAddress = userDTO.StreetAddress;
            user.City = userDTO.City;
            user.State = userDTO.State;
            user.PostalCode = userDTO.PostalCode;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
