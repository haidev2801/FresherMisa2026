using FresherMisa2026.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FresherMisa2026.Infrastructure.Implementations
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (ttl.HasValue)
                options.AbsoluteExpirationRelativeToNow = ttl;

            _cache.Set(key, value, options);

            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
