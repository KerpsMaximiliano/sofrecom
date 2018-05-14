using System;
using System.Collections.Generic;

namespace Sofco.Core.Cache
{
    public interface ICacheManager
    {
        T Get<T>(string cacheKey);

        T Get<T>(string cacheKey, Func<T> resolver, TimeSpan? cacheExpire = null);

        void Set<T>(string cacheKey, T result, TimeSpan? cacheExpire = null);

        IList<T> GetHashList<T>(string cacheKey);

        IList<T> GetHashList<T>(string cacheKey, Func<IList<T>> resolver, Func<T, string> getKey, TimeSpan cacheExpire);

        void SetHashList<T>(string cacheKey, IList<T> result, Func<T, string> getKey, TimeSpan cacheExpire);

        void DeletePatternKey(string pattern);
    }
}
