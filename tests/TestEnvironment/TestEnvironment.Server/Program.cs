using DosProtection.AspNetApi.Middleware;
using TestEnvironment.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICacheProvider, InMemoryCache>();

var powSection = builder.Configuration.GetSection("PowConfig");

builder.Services.AddStaticPowChallenging(null, config =>
{
    powSection.Bind(config);
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
