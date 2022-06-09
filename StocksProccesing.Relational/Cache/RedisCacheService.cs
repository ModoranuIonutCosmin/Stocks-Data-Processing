using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public void Delete<T>(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }

        public async Task<T> GetOrDefault<T>(string cacheKey)
        {
            string value;

            try
            {
                value = await _cache.GetStringAsync(cacheKey);
            }
            catch
            {
                return default;
            }

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task Set<T>(string cacheKey, T value, TimeSpan expiration)
        {
            string valueJson = JsonConvert.SerializeObject(value);

            try
            {
                await _cache.SetStringAsync(cacheKey, valueJson, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = expiration,
                    AbsoluteExpiration = DateTimeOffset.UtcNow.Add(expiration),
                    AbsoluteExpirationRelativeToNow = expiration
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
