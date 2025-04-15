namespace DosProtection.AspNetApi.Middleware;

public class RuntimePowData
{
    public int Difficulty { get; set; }
    public float CacheLifetimeMinutes { get; set; }
}

public interface IRuntimePowDataProvider
{
    public RuntimePowData GetPowData();
}
