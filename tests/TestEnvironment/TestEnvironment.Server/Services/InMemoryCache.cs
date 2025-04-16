using DosProtection.AspNetApi.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace TestEnvironment.Server.Services;

public class InMemoryCache : ICacheProvider, IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public Task WriteAsync(string? key, string? value, TimeSpan timeSpan)
    {
        if (key != null && value != null)
        {
            _cache.Set(key, value, timeSpan);
        }
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string? key)
    {
        if (key == null) return Task.FromResult<string?>(null);

        _ = _cache.TryGetValue(key, out var result);
        return Task.FromResult(result as string);
    }

    public Task<bool> Contains(string key)
    {
        var exists = _cache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }

    public Task RemoveAsync(string? key, string? value)
    {
        if (key != null)
        {
            _cache.Remove(key);
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}
