using TestEnvironment.Client.Configs;

namespace TestEnvironment.Client.Services;

public class StressTester(ApiRequester apiRequester, StressTestConfig stressTestConfig)
{
    public async Task Test()
    {
        Console.WriteLine($"Starting stress test for {stressTestConfig.Seconds} seconds. " +
                          $"RPS={stressTestConfig.RequestsPerSecond}, SolvePow={stressTestConfig.SolvePow}");
        
        int totalSeconds = stressTestConfig.Seconds;
        int rps = stressTestConfig.RequestsPerSecond;
        double delayMs = 1000.0 / rps;

        for (int second = 0; second < totalSeconds; second++)
        {
            for (int i = 0; i < rps; i++)
            {
                _ = stressTestConfig.SolvePow 
                    ? apiRequester.SendRequestWithPow() 
                    : apiRequester.SendRequestWithoutPow();

                await Task.Delay(TimeSpan.FromMilliseconds(delayMs));
            }
        }

        Console.WriteLine($"Stress test finished!");
    }
}
