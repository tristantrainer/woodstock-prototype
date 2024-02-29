using Microsoft.Extensions.DependencyInjection;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface IRepositoryFactory 
{
    TRepository Create<TRepository>() where TRepository : notnull;
}

internal class RepositoryFactory(IServiceScopeFactory serviceScopeFactory) : IRepositoryFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public TRepository Create<TRepository>() where TRepository : notnull
    {
        var scopedServiceProvider = _serviceScopeFactory.CreateScope();
        return scopedServiceProvider
            .ServiceProvider
            .GetRequiredService<TRepository>();
    }
}