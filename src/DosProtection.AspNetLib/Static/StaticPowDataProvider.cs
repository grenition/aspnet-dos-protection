using DosProtection.AspNetApi.Middleware;
using Microsoft.Extensions.Options;

namespace DosProtection.AspNetApi.Static;

public class StaticPowDataProvider(IOptions<PowStaticConfig> powStaticConfig) : IRuntimePowDataProvider
{
    private readonly RuntimePowData _runtimePowData = new RuntimePowData()
    {
        Difficulty = powStaticConfig.Value.Difficulty,
        CacheLifetimeSeconds = powStaticConfig.Value.CacheLifetimeSeconds,
        PoolSize = powStaticConfig.Value.PoolSize
    };

    public RuntimePowData GetPowData() => _runtimePowData;
}
