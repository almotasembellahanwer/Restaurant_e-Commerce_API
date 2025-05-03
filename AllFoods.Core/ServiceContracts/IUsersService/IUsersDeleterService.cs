using AllFoods.Core.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.IUsersService
{
    public interface IUsersDeleterService
    {
        Task<bool> DeleteUserByEmail(string email);

    }
}
