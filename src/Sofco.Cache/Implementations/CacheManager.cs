using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sofco.Cache.Interfaces;
using StackExchange.Redis;

namespace Sofco.Cache.Implementations
{
    public class CacheManager : ICacheManager
    {
        private readonly IDatabase redis;
        private readonly JsonSerializerSettings serializerSettings;

        public CacheManager(IDatabase redis)
        {
            this.redis = redis;

            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            serializerSettings.Converters.Add(new StringEnumConverter(true));

            serializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'" });
        }

        public T Get<T>(string cacheKey)
        {
            var cacheValue = redis.StringGet(cacheKey);

            return cacheValue.HasValue
                ? Deserialize<T>(cacheValue)
                : default(T);
        }

        public T Get<T>(string cacheKey, Func<T> resolver, TimeSpan? cacheExpire = null)
        {
            T result;

            var cache = Get<T>(cacheKey);

            if (cache != null)
            {
                result = cache;
            }
            else
            {
                result = resolver();

                if (result != null)
                {
                    Set(cacheKey, result, cacheExpire);
                }
            }

            return result;
        }

        public void Set<T>(string cacheKey, T result, TimeSpan? cacheExpire = null)
        {
            redis.StringSet(cacheKey, Serialize(result), cacheExpire);
        }

        private string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, serializerSettings);
        }

        private T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, serializerSettings);
        }
    }
}
