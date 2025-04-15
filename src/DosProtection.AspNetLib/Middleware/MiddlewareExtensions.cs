using DosProtection.AspNetApi.Dynamic;
using DosProtection.AspNetApi.Static;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DosProtection.AspNetApi.Middleware;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddStaticPowChallenging(
        this IServiceCollection services, 
        Func<IServiceProvider, ICacheProvider> cacheProvider,
        Action<PowStaticConfig> configure)
    {
        services.Configure(configure);
        services.AddScoped(cacheProvider);
        services.AddScoped<IRuntimePowDataProvider, StaticPowDataProvider>();
        
        return services;
    }

    public static IServiceCollection AddDynamicPowChallenging(
        this IServiceCollection services,
        Func<IServiceProvider, IServerStressProvider> serverStressProvider,
        Func<IServiceProvider, ICacheProvider> cacheProvider,
        Action<PowDynamicConfig> configure)
    {
        services.Configure(configure);
        services.AddScoped(serverStressProvider);
        services.AddScoped(cacheProvider);
        services.AddScoped<IRuntimePowDataProvider, DynamicPowDataProvider>();

        return services;
    }

    public static IApplicationBuilder UsePowChallenging(this IApplicationBuilder app)
    {
        app.UseMiddleware<PowChallengeMiddleware>();

        return app;
    }
}
