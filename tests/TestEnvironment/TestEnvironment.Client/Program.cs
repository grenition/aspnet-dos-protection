using Microsoft.Extensions.Configuration;
using TestEnvironment.Client.Configs;
using TestEnvironment.Client.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var serverConfig = configuration.GetSection("ServerConfig").Get<ServerConfig>();
var httpClient = new HttpClient { BaseAddress = new Uri(serverConfig!.Url!) };
var powClient = new PowHttpClient(httpClient);
var apiRequester = new ApiRequester(serverConfig, powClient, httpClient);

await apiRequester.SendRequestWithoutPow();
await apiRequester.SendRequestWithoutPow();
await apiRequester.SendRequestWithoutPow();
await apiRequester.SendRequestWithoutPow();
await apiRequester.SendRequestWithoutPow();

await apiRequester.SendRequestWithPow();
await apiRequester.SendRequestWithPow();
await apiRequester.SendRequestWithPow();
await apiRequester.SendRequestWithPow();
await apiRequester.SendRequestWithPow();
