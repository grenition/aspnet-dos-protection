using DosProtection.AspNetApi.Cache;
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
        services.AddScoped<IChallengePool, ChallengePool>();
        services.AddScoped<IRuntimePowDataProvider, StaticPowDataProvider>();
        services.AddScoped<PowChallengeMiddleware>();
        
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
