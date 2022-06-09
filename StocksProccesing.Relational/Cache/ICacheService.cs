using System;
using System.Threading.Tasks;

namespace StocksProccesing.Relational.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrDefault<T>(string cacheKey);
        Task Set<T>(string cacheKey, T value, TimeSpan expiration);
        void Delete<T>(string cacheKey);
    }
}
