#define UseCache

using KiriathSolutions.Woodstock.Infrastructure.Adapters;
using KiriathSolutions.Woodstock.Infrastructure.Data.Caches;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Infrastructure.HostedServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {   
        services
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .AddTransient<ISerializer, NewtonsoftAdapter>()
            .AddTransient<IRepositoryFactory, RepositoryFactory>()
            .AddHostedService<QueuedHostedService>()
            .AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        services
            .AddSingleton<IQueryCache, QueryCache>()
            .AddSingleton<ICacheUnitOfWork, CacheUnitOfWork>()
            .AddTransient<IAccountsCacheRepository, AccountsCacheRepository>()
            .AddTransient<ITransactionCategoryCacheRepository, TransactionCategoryCacheRepository>()
            .AddTransient<ITransactionCacheRepository, TransactionsCacheRepository>();

#if UseCache
        services
            .AddSingleton<IDataCache, DataCache>()
            .AddTransient<IAccountsRepository, AccountsLiteRepository>()
            .AddTransient<IAccountCategoriesRepository, AccountCategoriesLiteRepository>()
            .AddTransient<ITransactionCategoryRepository, TransactionCategoryLiteRepository>()
            .AddTransient<ITransactionRepository, TransactionLiteRepository>();
#else
        services
            .AddTransient<IAccountsRepository, AccountsRepository>();
            .AddTransient<ITransactionRepository, TransactionsRepository>()
            .AddTransient<ITransactionCategoryRepository, TransactionCategoriesRepository>()
            .AddTransient<IAccountCategoriesRepository, AccountCategoriesRepository>();
#endif

        return services;
    }
}