namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface IUnitOfWork
{
    IAccountsRepository Accounts { get; }
    IAccountCategoriesRepository AccountCategories { get; }

    ITransactionRepository Transactions { get; }
    ITransactionCategoryRepository TransactionCategories { get; }

    void SaveChanges();
    Task SaveChangesAsync();
}

internal sealed class UnitOfWork(IRepositoryFactory factory) : IUnitOfWork
{
    private readonly IRepositoryFactory _factory = factory;
    private readonly List<IRepository> _repositories = [];

    private IAccountsRepository? _accounts;
    public IAccountsRepository Accounts => _accounts ??= CreateRepository<IAccountsRepository>();

    private IAccountCategoriesRepository? _accountCategories;
    public IAccountCategoriesRepository AccountCategories => _accountCategories ??= CreateRepository<IAccountCategoriesRepository>();

    private ITransactionRepository? _transactions;
    public ITransactionRepository Transactions => _transactions ??= CreateRepository<ITransactionRepository>();

    private ITransactionCategoryRepository? _transactionCategories;
    public ITransactionCategoryRepository TransactionCategories => _transactionCategories ??= CreateRepository<ITransactionCategoryRepository>();

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
