namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface ICacheUnitOfWork
{
    IAccountsCacheRepository Accounts { get; }
    ITransactionCacheRepository Transactions { get; }
    ITransactionCategoryCacheRepository TransactionCategories { get; }

    void SaveChanges();
    Task SaveChangesAsync();
}

internal sealed class CacheUnitOfWork(IRepositoryFactory factory) : ICacheUnitOfWork
{
    private readonly IRepositoryFactory _factory = factory;
    private readonly List<IRepository> _repositories = [];

    private IAccountsCacheRepository? _accounts;
    public IAccountsCacheRepository Accounts => _accounts ??= CreateRepository<IAccountsCacheRepository>();

    private ITransactionCacheRepository? _transactions;
    public ITransactionCacheRepository Transactions => _transactions ??= CreateRepository<ITransactionCacheRepository>();

    private ITransactionCategoryCacheRepository? _transactionCategories;
    public ITransactionCategoryCacheRepository TransactionCategories => _transactionCategories ??= CreateRepository<ITransactionCategoryCacheRepository>();

    private T CreateRepository<T>() where T : notnull, IRepository 
    {
        var repository = _factory.Create<T>();
        _repositories.Add(repository);
        return repository;
    }

    public void SaveChanges() 
    {
        foreach (var repository in _repositories)
        {
            repository.SaveChanges();
        }
    }

    public Task SaveChangesAsync() 
    {
        return Task.WhenAll(
            _repositories
                .Select((repository) => repository.SaveChangesAsync())
                .ToArray()
        );
    }
}
