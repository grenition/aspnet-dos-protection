namespace DosProtection.AspNetApi.Dynamic;

public class PowDynamicConfig
{
    /// <summary>
    /// Maps Server stress (range: 0.0 - 1.0) to Difficculty 
    /// </summary>
    public Dictionary<float, int> DifficultyMap { get; set; } = new();
    public int CacheLifetimeMinutes { get; set; }
}
