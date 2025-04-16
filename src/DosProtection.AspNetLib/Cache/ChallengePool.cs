using System.Security.Cryptography;
using System.Text.Json;
using DosProtection.AspNetApi.Middleware;
using DosProtection.ProofOfWork;

namespace DosProtection.AspNetApi.Cache;

public class ChallengePool(
    ICacheProvider cacheProvider, 
    IRuntimePowDataProvider runtimePowDataProvider) : IChallengePool
{
    private const string SlotKeyPrefix = "POW_ChallengePool_Challenge-";
    
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    private readonly TimeSpan _lifetime = TimeSpan
        .FromSeconds(runtimePowDataProvider.GetPowData().CacheLifetimeSeconds);

    public async Task<(string? Challenge, string challengeJson)> AcquireChallengeAsync()
    {
        int slotIndex = GetRandomSlotIndex();
        string slotKey = $"{SlotKeyPrefix}{slotIndex}";

        var challengeJson = await cacheProvider.GetAsync(slotKey);

        if (string.IsNullOrEmpty(challengeJson))
        {
            var (cid, json) = GenerateChallengeWithIndex(slotIndex);
            challengeJson = json;

            await cacheProvider.WriteAsync(slotKey, challengeJson, _lifetime);
        }

        var statement = JsonSerializer.Deserialize<PowChallengeStatement>(challengeJson);
        if (statement?.Challenge == null)
        {
            var (cid, json) = GenerateChallengeWithIndex(slotIndex);
            challengeJson = json;
            await cacheProvider.WriteAsync(slotKey, challengeJson, _lifetime);
            statement = JsonSerializer.Deserialize<PowChallengeStatement>(challengeJson);
        }

        await cacheProvider.WriteAsync(statement!.Challenge, challengeJson, _lifetime);

        return (statement.Challenge, challengeJson);
    }

    public async Task ReleaseChallengeAsync(string challengeId, bool solved)
    {
        if (!solved)
            return;

        var parts = challengeId.Split('_', 2);
        if (parts.Length < 2)
            return;
        if (!int.TryParse(parts[0], out int slotIndex))
            return;

        var slotKey = $"{SlotKeyPrefix}{slotIndex}";
        var (cid, json) = GenerateChallengeWithIndex(slotIndex);
        
        await cacheProvider.WriteAsync(slotKey, json, _lifetime);
    }

    private (string ChallengeId, string SerializedJson) GenerateChallengeWithIndex(int index)
    {
        var statement = PowChallenge.GenerateChallenge(
            new PowChallengeConfig { Difficulty = runtimePowDataProvider.GetPowData().Difficulty }
        );
        
        statement.Challenge = $"{index}_{statement.Challenge}";
        var serialized = JsonSerializer.Serialize(statement);
        return (statement.Challenge, serialized);
    }

    private int GetRandomSlotIndex()
    {
        var bytes = new byte[4];
        _rng.GetBytes(bytes);
        int val = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
        return val % runtimePowDataProvider.GetPowData().PoolSize;
    }
}
