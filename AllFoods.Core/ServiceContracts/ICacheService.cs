using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
        Task RemoveByPatternAsync(string pattern);
        Task InvalidateProductCachesAsync(Guid? productID = null);
        Task<bool> ExistsAsync(string key);
    }
}
