namespace DosProtection.AspNetApi.Cache;

public interface IChallengePool
{
    Task<(string? Challenge, string challengeJson)> AcquireChallengeAsync();
    Task ReleaseChallengeAsync(string challengeId, bool solved);
}
