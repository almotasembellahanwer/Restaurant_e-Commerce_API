using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.RepositoryContracts
{
    public interface IAccountRepository
    {
        Task<bool> IsUniqueUser(string userName);
        Task<ApplicationUser> GetUser(Expression<Func<ApplicationUser,bool>> pridicates);
        Task<IEnumerable<RefreshToken>> GetAllRefreshTokens(string userID);
        Task<RefreshToken> GetRefreshToken(Expression<Func<RefreshToken, bool>> pridicates);
        Task<int> MarkAllTokenInChainAsInvalid(string userID, string jwtTokenID);
        Task<string> CreateNewRefreshToken(string userID, string jwtTokenID);
        Task RemoveRenge(string userID);

        Task Save();
    }
}
