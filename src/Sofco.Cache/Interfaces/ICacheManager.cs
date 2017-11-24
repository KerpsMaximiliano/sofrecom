using System;

namespace Sofco.Cache.Interfaces
{
    public interface ICacheManager
    {
        T Get<T>(string cacheKey);
        T Get<T>(string cacheKey, Func<T> resolver, TimeSpan? cacheExpire = null);
        void Set<T>(string cacheKey, T result, TimeSpan? cacheExpire = null);
    }
}
