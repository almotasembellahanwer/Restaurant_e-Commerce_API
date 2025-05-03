using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.UserDTO;
using AllFoods.Core.ServiceContracts.IUsersService;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllFoods.Core.Services.UsersService
{
    public class UsersUpdaterService : IUsersUpdaterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersUpdaterService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }



        public async Task<bool> UpdateUser(UserUpdatDTO? userUpdateDTO)
        {
            if (userUpdateDTO is null)
            {
                return false;
            }
            var user = await _userManager.FindByIdAsync(userUpdateDTO.Id);
            if (user == null)
            {
                return false;
            }
            user.Name = userUpdateDTO.Name;
            user.StreetAddress = userUpdateDTO.StreetAddress;
            user.City = userUpdateDTO.City;
            user.State = userUpdateDTO.State;
            user.PostalCode = userUpdateDTO.PostalCode;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
