using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.UserDTO;
using AllFoods.Core.ServiceContracts.IUsersService;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllFoods.Core.Services.UsersService
{
    public class UsersGetterService : IUsersGetterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersGetterService(UserManager<ApplicationUser> userManager, IMapper mapper)
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
            string? role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if(role is not null)
                userDTO.Role = role;
            return userDTO;
        }

       

        

       
    }
}
