using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.UserDTO;
using AllFoods.Core.ServiceContracts.IUsersService;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllFoods.Core.Services.UsersService
{
    public class UsersLockerService : IUsersLockerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersLockerService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }



        public async Task<bool> LockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return false;

            // Check if the user is admin
            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                return false;
            }
            // Immediately lock with max date and enable lockout
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            // Invalidate all existing tokens by updating the security stamp
            await _userManager.UpdateSecurityStampAsync(user);
            return true;
        }

        public async Task<bool> UnlockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            return true;
        }
    }
}
