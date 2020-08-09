using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweekBook.Services
{
    public interface IResponseCacheService
    {
        //put the data 
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeSpan);

        Task<string> GetCacheResponseAsync(string cacheKey);
        //retrive the data
    }
}
