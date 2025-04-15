namespace DosProtection.AspNetApi.Middleware;

public interface ICacheProvider
{
    public Task WriteAsync(string key, TimeSpan timeSpan);
    public Task<bool> Contains(string key);
}
