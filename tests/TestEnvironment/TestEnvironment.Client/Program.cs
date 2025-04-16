using Microsoft.Extensions.Configuration;
using TestEnvironment.Client.Configs;
using TestEnvironment.Client.Services;

var configFilePath = args.FirstOrDefault();
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(configFilePath ?? "Config.json")
    .Build();

var serverConfig = configuration.GetSection("ServerConfig").Get<ServerConfig>();
var stressTestConfig = configuration.GetSection("StressTestConfig").Get<StressTestConfig>();
var httpClient = new HttpClient { BaseAddress = new Uri(serverConfig!.Url!) };
var powClient = new PowHttpClient(httpClient);
var apiRequester = new ApiRequester(serverConfig, powClient, httpClient);
var stressTester = new StressTester(apiRequester, stressTestConfig!);

await stressTester.Test();