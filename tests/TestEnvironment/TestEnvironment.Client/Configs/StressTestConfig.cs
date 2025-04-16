namespace TestEnvironment.Client.Configs;

public class StressTestConfig
{
    public int Seconds { get; set; }
    public bool SolvePow { get; set; }
    public int RequestsPerSecond { get; set; }
}
