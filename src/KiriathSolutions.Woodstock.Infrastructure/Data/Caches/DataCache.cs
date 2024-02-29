using KiriathSolutions.Woodstock.Contracts.Interfaces;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Adapters;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

public interface IDataCache 
{
    Task<Account[]> GetAccountsAsync();
    Task SaveAccountsAsync(Account[] accounts);

    Task<AccountCategory[]> GetAccountCategoriesAsync();
    Task SaveAccountCategoriesAsync(AccountCategory[] accounts);

    Task<Transaction[]> GetTransactionsAsync();
    Task SaveTransactionsAsync(Transaction[] accounts);

    Task<TransactionCategory[]> GetTransactionCategoriesAsync();
    Task SaveTransactionCategoriesAsync(TransactionCategory[] accounts);
}

internal sealed class DataCache(IDataCacheSettings settings, ISerializer serializer) : IDataCache
{
    private readonly IDataCacheSettings _settings = settings;
    private readonly ISerializer _serializer = serializer;

    #region Accounts

    private readonly object _accountsLock = new();

    private DirectoryAndFilePath GetAccountsPath()  
        => new(@$"{_settings.DirectoryPath}", "accounts.json");

    public Task<Account[]> GetAccountsAsync() {
        var path = GetAccountsPath();

        lock(_accountsLock) {
            return Task.FromResult(GetCachedData<Account[]>(path) ?? []);
        }
    }

    public Task SaveAccountsAsync(Account[] accounts) {
        var path = GetAccountsPath();

        lock(_accountsLock) {
            CacheData(accounts, path);
        }

        return Task.CompletedTask;
    }

    #endregion Accounts

    #region Account Categories

    private readonly object _accountCategoriesLock = new();

    private DirectoryAndFilePath GetAccountCategoriesPath()  
        => new(_settings.DirectoryPath, "accountCategories.json");

    public Task<AccountCategory[]> GetAccountCategoriesAsync() {
        var path = GetAccountCategoriesPath();

        lock(_accountCategoriesLock) {
            return Task.FromResult(GetCachedData<AccountCategory[]>(path) ?? []);
        }
    }

    public Task SaveAccountCategoriesAsync(AccountCategory[] accounts) {
        var path = GetAccountCategoriesPath();

        lock(_accountCategoriesLock) {
            CacheData(accounts, path);
        }

        return Task.CompletedTask;
    }

    #endregion Account Categories

    #region Transactions

    private readonly object _transactionsLock = new();

    private DirectoryAndFilePath GetTransactionsPath()  
        => new(_settings.DirectoryPath, "transactions.json");

    public Task<Transaction[]> GetTransactionsAsync() {
        var path = GetTransactionsPath();

        lock(_transactionsLock) {
            return Task.FromResult(GetCachedData<Transaction[]>(path) ?? []);
        }
    }

    public Task SaveTransactionsAsync(Transaction[] transactions) {
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
        => new(_settings.DirectoryPath, "transaction-categories.json");

    public Task<TransactionCategory[]> GetTransactionCategoriesAsync() {
        var path = GetTransactionCategoriesPath();

        lock(_transactionCategoriesLock) {
            return Task.FromResult(GetCachedData<TransactionCategory[]>(path) ?? []);
        }
    }

    public Task SaveTransactionCategoriesAsync(TransactionCategory[] categories) {
        var path = GetTransactionCategoriesPath();

        lock(_transactionCategoriesLock) {
            CacheData(categories, path);
        }

        return Task.CompletedTask;
    }

    #endregion TransactionCategories

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
    
public class CachedData<T>
{
    public double Expires { get; set; }
    public T Data { get; set; } = default!;
}

internal record DirectoryAndFilePath(string Directory, string FileName) 
{
    public string FullPath => $@"{Directory}\{FileName}";
}