using TestEnvironment.Client.Configs;

namespace TestEnvironment.Client.Services
{
    public class StressTester(ApiRequester apiRequester, StressTestConfig stressTestConfig)
    {
        public async Task Test()
        {
            Console.WriteLine($"Starting stress test for {stressTestConfig.Seconds} seconds. " +
                              $"RPS={stressTestConfig.RequestsPerSecond}, SolvePow={stressTestConfig.SolvePow}");
            
            int totalSeconds = stressTestConfig.Seconds;
            int rps = stressTestConfig.RequestsPerSecond;
            double delayMs = 1000.0 / rps;
            var semaphore = new SemaphoreSlim(stressTestConfig.MaxParallelOperationsCount);

            var startTime = DateTime.UtcNow;
            
            for (int second = 0; second < totalSeconds; second++)
            {
                var secondTasks = new List<Task>();

                for (int i = 0; i < rps; i++)
                {
                    await semaphore.WaitAsync();
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            if (stressTestConfig.SolvePow)
                                await apiRequester.SendRequestWithPow();
                            else
                                await apiRequester.SendRequestWithoutPow();
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
                    secondTasks.Add(task);

                    await Task.Delay(TimeSpan.FromMilliseconds(delayMs));
                }

                await Task.WhenAll(secondTasks);
            }
            
            Console.WriteLine("Stress test finished! All tasks completed!");
        }
    }
}
