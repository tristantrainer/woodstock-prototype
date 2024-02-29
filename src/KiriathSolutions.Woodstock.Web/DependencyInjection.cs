using KiriathSolutions.Woodstock.Contracts.Interfaces;
using KiriathSolutions.Woodstock.Web.Hubs;
using KiriathSolutions.Woodstock.Web.Infrastructure;
using KiriathSolutions.Woodstock.Web.Settings;

namespace Microsoft.Extensions.DependencyInjection;

internal static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {   
        services.AddSingleton<IDataCacheSettings, DataCacheSettings>();
        services.AddSingleton<IQueryCacheSettings, QueryCacheSettings>();
        services.AddSingleton<IHostedServiceOptions, HostedServiceOptions>();
        services.AddTransient<ICacheSubscriptionService, CacheSubscriptionService>();
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
}