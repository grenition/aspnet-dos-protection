using DosProtection.AspNetApi.Middleware;
using Microsoft.Extensions.Caching.Memory;

namespace TestEnvironment.Server.Services
{
    public class InMemoryCache : ICacheProvider
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public Task WriteAsync(string? key, TimeSpan timeSpan)
        {
            _cache.Set(key!, true, timeSpan);
            return Task.CompletedTask;
        }

        public Task<bool> Contains(string key)
        {
            var exists = _cache.TryGetValue(key, out _);
            return Task.FromResult(exists);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
