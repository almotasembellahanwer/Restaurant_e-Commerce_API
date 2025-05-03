using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.UserDTO;
using AllFoods.Core.ServiceContracts.IUsersService;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllFoods.Core.Services.UsersService
{
    public class UsersDeleterService : IUsersDeleterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersDeleterService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }



        public async Task<bool> DeleteUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
