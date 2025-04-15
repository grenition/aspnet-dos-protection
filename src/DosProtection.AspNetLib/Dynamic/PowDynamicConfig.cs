namespace DosProtection.AspNetApi.Dynamic;

public class PowDynamicConfig
{
    public Dictionary<float, int> DifficultyMap { get; set; } = new();
    public int CacheLifetimeMinutes { get; set; }
}
