using System.Text.Json;
using DosProtection.ProofOfWork;
using Microsoft.AspNetCore.Http;

namespace DosProtection.AspNetApi.Middleware;

public class PowChallengeMiddleware(
    IRuntimePowDataProvider runtimePowDataProvider,
    ICacheProvider cacheProvider) : IMiddleware
{
    private const string ChallengeHeader = "PoW-Challenge";
    private const string NonceHeader = "PoW-Nonce";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var runtimeData = runtimePowDataProvider.GetPowData();
        var challengeValue = context.Request.Headers[ChallengeHeader].FirstOrDefault();
        var nonceValue = context.Request.Headers[NonceHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(challengeValue) || string.IsNullOrWhiteSpace(nonceValue))
        {
            await SendChallengeAsync(context, runtimeData.Difficulty, runtimeData);
            return;
        }

        if (!int.TryParse(nonceValue, out var nonce))
        {
            await SendChallengeAsync(context, runtimeData.Difficulty, runtimeData);
            return;
        }

        var statement = new PowChallengeStatement
        {
            Challenge = challengeValue,
            Difficulty = runtimeData.Difficulty
        };
        var solution = new PowChallengeSolution
        {
            Challenge = challengeValue,
            Nonce = nonce
        };

        var isValid = PowChallenge.ValidateChallenge(statement, solution);
        if (!isValid)
        {
            await SendChallengeAsync(context, runtimeData.Difficulty, runtimeData);
            return;
        }
        
        var inCache = await cacheProvider.Contains(challengeValue);
        if (!inCache)
        {
            await SendChallengeAsync(context, runtimeData.Difficulty, runtimeData);
            return;
        }

        await next(context);
    }

    private async Task SendChallengeAsync(HttpContext context, int difficulty, RuntimePowData runtimeData)
    {
        var statement = PowChallenge.GenerateChallenge(
            new PowChallengeConfig { Difficulty = difficulty }
        );

        await cacheProvider.WriteAsync(statement.Challenge, TimeSpan.FromMinutes(runtimeData.CacheLifetimeMinutes));

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(statement);
        await context.Response.WriteAsync(json);
    }
}
