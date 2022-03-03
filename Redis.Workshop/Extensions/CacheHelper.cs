using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Redis.Workshop.Extensions
{
    public static class CacheHelper
    {
        public static TResult GetOrCreate<TResult>(this IDistributedCache distributedCache, string cacheKey, Func<TResult> function,
            DistributedCacheEntryOptions cacheEntryOptions = null)
            where TResult : class
        {
            if(cacheEntryOptions == null)
            {
                cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    //SlidingExpiration = TimeSpan.FromSeconds(5),
                    AbsoluteExpiration = DateTime.Now.AddSeconds(5)
                };
            }

            var cacheValue = distributedCache.GetString(cacheKey);

            if (cacheValue == null)
            {
                var result = function();

                distributedCache.SetString(cacheKey, JsonConvert.SerializeObject(result), cacheEntryOptions);

                return result;
            }
            else
            {
                return JsonConvert.DeserializeObject<TResult>(cacheValue);
            }
        }

    }
}
