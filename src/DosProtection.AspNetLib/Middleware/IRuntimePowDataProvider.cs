namespace DosProtection.AspNetApi.Middleware;

public class RuntimePowData
{
    public int Difficulty { get; set; }
    public int PoolSize { get; set; }
    public float CacheLifetimeSeconds { get; set; }
}

public interface IRuntimePowDataProvider
{
    public RuntimePowData GetPowData();
}
