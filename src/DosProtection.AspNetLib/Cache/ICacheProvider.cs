namespace DosProtection.AspNetApi.Cache;

public interface ICacheProvider
{
    public Task WriteAsync(string? key, string? value, TimeSpan timeSpan);
    public Task RemoveAsync(string? key, string? value);
    public Task<string?> GetAsync(string? key);
    public Task<bool> Contains(string key);
}
