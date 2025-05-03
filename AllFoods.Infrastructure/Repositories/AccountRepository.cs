using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<ApplicationUser> GetUser(Expression<Func<ApplicationUser, bool>> pridicates)
        {
           ApplicationUser? user = await _db.ApplicationUsers.FirstOrDefaultAsync(pridicates);
            if (user == null)
                throw new ArgumentNullException("user not found");
            return user;
        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if(user is null)
            {
                return true;
            }

            return false;
        }

        public async Task<string> CreateNewRefreshToken(string userID, string jwtTokenID)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserID = userID,
                JwtTokenID = jwtTokenID,
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
                ExpiredAt = DateTime.UtcNow.AddDays(1)
            };
            // Add refresh token to database
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
            // return Refresh_Token properity
            return refreshToken.Refresh_Token;
        }

        public async Task<RefreshToken> GetRefreshToken(Expression<Func<RefreshToken, bool>> pridicates)
        {
            RefreshToken? refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(pridicates);
            if (refreshToken == null)
                throw new ArgumentNullException("refresh token not found");
            return refreshToken;
        }
        public async Task<int> MarkAllTokenInChainAsInvalid(string userID, string jwtTokenID)
        {
            return await _db.RefreshTokens.Where(temp=>temp.UserID == userID && temp.JwtTokenID == jwtTokenID).ExecuteUpdateAsync(temp=>temp.SetProperty(refreshToken=>refreshToken.IsValid,false));
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetAllRefreshTokens(string userID)
        {
           IEnumerable<RefreshToken> refreshTokenList = await _db.RefreshTokens
                .AsNoTracking()
                .Where(temp=>temp.UserID == userID)
                .ToListAsync();
            return refreshTokenList;
        }

        public async Task RemoveRenge(string userID)
        {
           IEnumerable<RefreshToken> refreshTokenList = await GetAllRefreshTokens(userID);
           _db.RefreshTokens.RemoveRange(refreshTokenList);
        }
    }
}
