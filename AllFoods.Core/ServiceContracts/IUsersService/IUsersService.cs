using AllFoods.Core.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.IUsersService
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetAllUsers();
        Task<UserDTO?> GetUserDetails(string? userID);
        Task<bool> UpdateUser(string userID, UserDTO userDTO);
        Task<bool> DeleteUserByEmailAsync(string email);
        Task<bool> LockUser(string email);
        Task<bool> UnlockUser(string email);




    }
}
