using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweekBook.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeSpan)
        {
            if(response == null)
            {
                return;
            }

            var serializedResponse = JsonConvert.SerializeObject(response);
            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = timeSpan
            });
        }

        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
            return String.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
        }
    }
}
