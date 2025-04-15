using DosProtection.AspNetApi.Dynamic;
using DosProtection.AspNetApi.Static;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DosProtection.AspNetApi.Middleware;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddStaticPowChallenging(
        this IServiceCollection services, 
        Func<IServiceProvider, ICacheProvider>? cacheProvider,
        Action<PowStaticConfig>? configure)
    {
        services.AddScoped<IRuntimePowDataProvider, StaticPowDataProvider>();
        services.AddScoped<PowChallengeMiddleware>();
        
        if(cacheProvider != null)
            services.AddScoped(cacheProvider);
        if(configure != null)
            services.Configure(configure);
        
        return services;
    }

    public static IServiceCollection AddDynamicPowChallenging(
        this IServiceCollection services,
        Func<IServiceProvider, IServerStressProvider>? serverStressProvider,
        Func<IServiceProvider, ICacheProvider>? cacheProvider,
        Action<PowDynamicConfig>? configure)
    {
        services.AddScoped<IRuntimePowDataProvider, DynamicPowDataProvider>();
        services.AddScoped<PowChallengeMiddleware>();
        if(serverStressProvider != null)
            services.AddScoped(serverStressProvider);
        if(cacheProvider != null)
            services.AddScoped(cacheProvider);
        if(configure != null)
            services.Configure(configure);

        return services;
    }

    public static IApplicationBuilder UsePowChallenging(this IApplicationBuilder app)
    {
        app.UseMiddleware<PowChallengeMiddleware>();

        return app;
    }
}
