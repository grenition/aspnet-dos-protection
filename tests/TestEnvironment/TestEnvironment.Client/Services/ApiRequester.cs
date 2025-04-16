using TestEnvironment.Client.Configs;

namespace TestEnvironment.Client.Services;

public class ApiRequester(ServerConfig serverConfig, PowHttpClient powHttpClient, HttpClient httpClient)
{
    private int _requestWithoutPowCount = 0;
    private int _requestWithPowCount = 0;
    
    public async Task SendRequestWithoutPow()
    {
        var result = await httpClient.PostAsync(serverConfig.TestEndpoint!, null);
        _requestWithoutPowCount++;
        Console.WriteLine($"Completed request! {GetRequestsStats()}; Response: {await result.Content.ReadAsStringAsync()}");
    }
    
    public async Task SendRequestWithPow()
    {
        var result = await powHttpClient.PostWithPowAsync(serverConfig.TestEndpoint!, null);
        _requestWithPowCount++;
        Console.WriteLine($"Completed POW request! {GetRequestsStats()}; Response: {await result.Content.ReadAsStringAsync()}");

    }

    private string GetRequestsStats() => $"Stats: Without pow: {_requestWithoutPowCount}, " +
                                         $"With Pow: {_requestWithPowCount}, " +
                                         $"Total: {_requestWithoutPowCount + _requestWithPowCount}";
}
