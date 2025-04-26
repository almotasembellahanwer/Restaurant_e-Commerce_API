using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.AccountDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.IUsersService
{
    public interface IAccountService
    {
        Task<bool> IsUniqueUser(string userName);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<AccountDTO> Register(RegistrationRequestDTO registerRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
        Task RevokeRefreshToken (TokenDTO tokenDTO);


    }
}
