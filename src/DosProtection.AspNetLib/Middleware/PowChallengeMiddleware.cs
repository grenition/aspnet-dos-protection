using DosProtection.AspNetApi.Cache;
using DosProtection.ProofOfWork;
using Microsoft.AspNetCore.Http;

namespace DosProtection.AspNetApi.Middleware;

public class PowChallengeMiddleware(
    IRuntimePowDataProvider runtimeDataProvider, 
    IChallengePool challengePool) : IMiddleware
{
    private const string ChallengeHeader = "PoW-Challenge";
    private const string NonceHeader = "PoW-Nonce";

    private static readonly SemaphoreSlim ChallengeSemaphore = new(5, 5);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var runtimeData = runtimeDataProvider.GetPowData();
        var challengeValue = context.Request.Headers[ChallengeHeader].FirstOrDefault();
        var nonceValue = context.Request.Headers[NonceHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(challengeValue) || string.IsNullOrWhiteSpace(nonceValue))
        {
            await SendChallengeAsync(context);
            return;
        }

        if (!int.TryParse(nonceValue, out var nonce))
        {
            await SendChallengeAsync(context);
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
            await SendChallengeAsync(context);
            return;
        }
        
        await challengePool.ReleaseChallengeAsync(challengeValue, solved: true);

        await next(context);
    }

    private async Task SendChallengeAsync(HttpContext context)
    {
        await ChallengeSemaphore.WaitAsync();
        try
        {
            var (challengeId, serialized) = await challengePool.AcquireChallengeAsync();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(serialized);
        }
        finally
        {
            ChallengeSemaphore.Release();
        }
    }
}
