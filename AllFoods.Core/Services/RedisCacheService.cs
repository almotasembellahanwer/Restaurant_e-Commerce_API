using AllFoods.Core.ServiceContracts;
using AllFoods.Core.Settinges;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AllFoods.Core.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _defaultOptions;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            _logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            // Get chache value by key
            var cacheDate = await _cache.GetStringAsync(key);
            if (cacheDate is not null)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(cacheDate)!;
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Failed to deserialize cached data for key {Key}",key);
                }

            }
                

            var data = await factory();
            if(factory is not null)
            {
                if(key.Length > 256)
                {
                    _logger.LogWarning("Key length exceeds 256 characters. Key: {Key}", key);
                    key = $"products_{key.GetHashCode()}";
                }
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(data),
                    expiry.HasValue ? new DistributedCacheEntryOptions
                    { AbsoluteExpirationRelativeToNow = expiry } : _defaultOptions
                    );
            }
            return data;
        }
        public async Task RemoveByPatternAsync(string pattern)
        {
            await _cache.RemoveAsync(pattern);
        }
        public async Task InvalidateProductCachesAsync(Guid? productID = null)
        {
            // Remove all product lists
            await RemoveByPatternAsync(CacheSettings.ProductsPattern());

            // Remove specific product cache if ID provided
            if (productID.HasValue)
            {
                await _cache.RemoveAsync(CacheSettings.ProductKey(productID));
            } 
            
        }
        public async Task<bool> ExistsAsync(string key)
        {
            string? keyExist = await _cache.GetStringAsync(key);
            return keyExist is not null;
        }
    }
}
