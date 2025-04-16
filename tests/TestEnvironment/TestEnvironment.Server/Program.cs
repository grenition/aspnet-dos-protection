using DosProtection.AspNetApi.Middleware;
using TestEnvironment.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICacheProvider, InMemoryCache>();

var stressAnalyzer = new StressAnalyzer();

builder.Services.AddStaticPowChallenging(null, config =>
{
    config.Difficulty = 10;
    config.CacheLifetimeMinutes = 2;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UsePowChallenging();

app.MapPost("/test", () => Results.Ok(new
{
    Message = "Welcome!"
}));

app.Run();
