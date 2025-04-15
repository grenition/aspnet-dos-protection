using DosProtection.AspNetApi.Middleware;
using Microsoft.Extensions.Options;

namespace DosProtection.AspNetApi.Dynamic;

public class DynamicPowDataProvider(
    IOptions<PowDynamicConfig> dynamicConfig,
    IServerStressProvider serverStressProvider) : IRuntimePowDataProvider
{
    private readonly RuntimePowData _runtimePowData = new();
    
    public RuntimePowData GetPowData()
    {
        var config = dynamicConfig.Value;
        var stress = serverStressProvider.GetStress();
        var map = config.DifficultyMap;

        var matchedKey = map
            .Where(kv => kv.Key <= stress)
            .MaxBy(kv => kv.Key);

        _runtimePowData.Difficulty = matchedKey.Value;
        _runtimePowData.CacheLifetimeMinutes = config.CacheLifetimeMinutes;

        return _runtimePowData;
    }
}
