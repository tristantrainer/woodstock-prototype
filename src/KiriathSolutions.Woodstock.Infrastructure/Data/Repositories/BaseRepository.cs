using KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

internal abstract class BaseLiteRepository(IDataCache dataCache) : IRepository
{
    protected IDataCache Cache { get; private set; } = dataCache;

    public void SaveChanges() { }
    public Task SaveChangesAsync() { 
        return Task.CompletedTask;
    }
}

internal abstract class BaseCacheRepository(IQueryCache dataCache) : IRepository
{
    protected IQueryCache Cache { get; private set; } = dataCache;

    public void SaveChanges() { }
    public Task SaveChangesAsync() { 
        return Task.CompletedTask;
    }
}

internal abstract class BaseRepository : IRepository
{
    public void SaveChanges() { }
    public Task SaveChangesAsync() { 
        return Task.CompletedTask;
    }
}

public interface IRepository
{
    void SaveChanges();
    Task SaveChangesAsync();
}
