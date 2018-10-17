﻿namespace CarDealer.Web.Infrastructure.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;

    public static class DistributedCacheExtensions
    {
        public static async Task<IDistributedCache> SetSerializableObject(this IDistributedCache cache, string key, object value, TimeSpan expiration)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var serializedObject = JsonConvert.SerializeObject(value, jsonSettings);

            await cache.SetStringAsync(key, serializedObject, cacheOptions);

            return cache;
        }
    }
}
