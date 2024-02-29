using KiriathSolutions.Woodstock.Contracts.Interfaces;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Infrastructure.Adapters;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

public interface IQueryCache 
{
    Task<CachedAccount[]> GetAccountsAsync();
    Task UpdateAccountsAsync(CachedAccount[] accounts);

    Task<CachedTransaction[]> GetTransactionsAsync();
    Task UpdateTransactionsAsync(CachedTransaction[] transactions);

    Task<CachedTransactionCategory[]> GetTransactionCategoriesAsync();
    Task UpdateTransactionCategoriesAsync(CachedTransactionCategory[] transactions);
}

internal sealed class QueryCache(IQueryCacheSettings settings, ISerializer serializer) : IQueryCache
{
    private readonly IQueryCacheSettings _settings = settings;
    private readonly ISerializer _serializer = serializer;

    #region Accounts

    private readonly object _accountsLock = new();

    private DirectoryAndFilePath GetAccountsPath()  
        => new(@$"{_settings.DirectoryPath}\", "accounts.json");

    public Task<CachedAccount[]> GetAccountsAsync() {
        var path = GetAccountsPath();

        lock(_accountsLock) {
            return Task.FromResult(GetCachedData<CachedAccount[]>(path) ?? []);
        }
    }

    public Task UpdateAccountsAsync(CachedAccount[] accounts) {
        var path = GetAccountsPath();

        lock(_accountsLock) {
            CacheData(accounts, path);
        }

        return Task.CompletedTask;
    }

    #endregion Accounts

    #region Transactions

    private readonly object _transactionsLock = new();

    private DirectoryAndFilePath GetTransactionsPath()  
        => new(@$"{_settings.DirectoryPath}\", "transactions.json");

    public Task<CachedTransaction[]> GetTransactionsAsync() {
        var path = GetTransactionsPath();

        lock(_transactionsLock) {
            return Task.FromResult(GetCachedData<CachedTransaction[]>(path) ?? []);
        }
    }

    public Task UpdateTransactionsAsync(CachedTransaction[] transactions) {
        var path = GetTransactionsPath();

        lock(_transactionsLock) {
            CacheData(transactions, path);
        }

        return Task.CompletedTask;
    }

    #endregion Transactions

    #region Transaction Categories

    private readonly object _transactionCategoriesLock = new();

    private DirectoryAndFilePath GetTransactionCategoriesPath()  
        => new(@$"{_settings.DirectoryPath}\", "transaction-categories.json");

    public Task<CachedTransactionCategory[]> GetTransactionCategoriesAsync() {
        var path = GetTransactionCategoriesPath();

        lock(_transactionCategoriesLock) {
            return Task.FromResult(GetCachedData<CachedTransactionCategory[]>(path) ?? []);
        }
    }

    public Task UpdateTransactionCategoriesAsync(CachedTransactionCategory[] transactions) {
        var path = GetTransactionCategoriesPath();

        lock(_transactionCategoriesLock) {
            CacheData(transactions, path);
        }

        return Task.CompletedTask;
    }

    #endregion Transactions

    #region Access

    private T? GetCachedData<T>(DirectoryAndFilePath path) 
    {
        if(File.Exists(path.FullPath) is false) 
            return default;

        var fileContents = File.ReadAllText(path.FullPath);

        var cache = _serializer.Deserialize<CachedData<T>>(fileContents);

        if(cache is null)
            return default;

        return cache.Data;
    }

    private void CacheData<T>(T items, DirectoryAndFilePath path) 
    {
        if(Directory.Exists(path.Directory) is false)
        {
            Directory.CreateDirectory(path.Directory);
        }

        var expires = (DateTime.UtcNow.Add(TimeSpan.FromMinutes(_settings.LifespanInMinutes)) - DateTime.UnixEpoch).TotalSeconds;

        File.WriteAllText(path.FullPath,_serializer.Serialize(new CachedData<T>
        {
            Data = items,
            Expires = expires,
        }));
    }

    #endregion Access
}