using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface IAccountsRepository : IRepository
{
    Task AddAsync(Account account);
    Task<bool> UpdateAsync(Account account);
    Task<bool> RemoveAsync(Guid accountId);
    Task<Account[]> GetAllAsync();
    Task<Account?> GetByPublicIdAsync(Guid id);
}

public interface IAccountsCacheRepository : IRepository 
{
    Task SaveAsync(CachedAccount[] accounts);
    Task AddAsync(CachedAccount account);
    Task<bool> UpdateAsync(CachedAccount account);
    Task<bool> RemoveAsync(Guid accountId);
    Task<CachedAccount[]> GetAllAsync();
    Task<CachedAccount?> GetByIdAsync(Guid id);
}

internal sealed class AccountsRepository : BaseRepository, IAccountsRepository
{
    public Task AddAsync(Account account)
    {
        return Task.CompletedTask;
    }

    public Task<Account[]> GetAllAsync()
    {
        return Task.FromResult<Account[]>([]);
    }

    public Task<Account?> GetByPublicIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Account account)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AccountsLiteRepository(IDataCache dataCache) : BaseLiteRepository(dataCache), IAccountsRepository
{
    public async Task AddAsync(Account account)
    {
        var accounts = await GetAllAsync();

        var lastId = accounts.Length > 0 ? accounts.Max((record) => record.Id) : 0;

        var toSave = new Account {
            Name = account.Name,
            PublicId = account.PublicId,
            Balance = account.Balance,
            CategoryId = account.CategoryId,
            Id = lastId + 1,
        };

        await Cache.SaveAccountsAsync([..accounts, toSave]);
    }

    public Task<Account[]> GetAllAsync()
    {
        return Cache.GetAccountsAsync();
    }

    public async Task<Account?> GetByPublicIdAsync(Guid publicId)
    {
        var accounts = await GetAllAsync();

        return accounts.FirstOrDefault((account) => account.PublicId == publicId);
    }

    public async Task<bool> RemoveAsync(Guid accountId)
    {
        var currentAccounts = await GetAllAsync();

        var updatedAccounts = currentAccounts
            .Where((record) => record.PublicId != accountId)
            .ToArray();

        await Cache.SaveAccountsAsync(updatedAccounts);

        return updatedAccounts.Length != currentAccounts.Length;
    }

    public async Task<bool> UpdateAsync(Account account)
    {
        var currentAccounts = await GetAllAsync();

        var existingAccount = currentAccounts
            .FirstOrDefault((record) => record.PublicId == account.PublicId);

        if(existingAccount is null)
            return false;

        var otherAccounts = currentAccounts
            .Where((record) => record.PublicId != account.PublicId)
            .ToArray();

        await Cache.SaveAccountsAsync([.. otherAccounts, existingAccount with { Name = account.Name, Balance = account.Balance, CategoryId = account.CategoryId, }]);

        return true;
    }
}

internal sealed class AccountsCacheRepository(IQueryCache queryCache) : BaseCacheRepository(queryCache), IAccountsCacheRepository
{
    public async Task AddAsync(CachedAccount account)
    {
        var accounts = await GetAllAsync();
        await Cache.UpdateAccountsAsync([..accounts, account]);
    }

    public Task<CachedAccount[]> GetAllAsync()
    {
        return Cache.GetAccountsAsync();
    }

    public async Task<CachedAccount?> GetByIdAsync(Guid id)
    {
        var accounts = await GetAllAsync();

        return accounts.FirstOrDefault((account) => account.PublicId == id);
    }

    public async Task<bool> RemoveAsync(Guid accountId)
    {
        var currentAccounts = await GetAllAsync();

        var updatedAccounts = currentAccounts
            .Where((record) => record.PublicId != accountId)
            .ToArray();

        await Cache.UpdateAccountsAsync(updatedAccounts);

        return updatedAccounts.Length != currentAccounts.Length;
    }

    public async Task SaveAsync(CachedAccount[] accounts)
    {
        await Cache.UpdateAccountsAsync(accounts);
    }

    public async Task<bool> UpdateAsync(CachedAccount account)
    {
        var currentAccounts = await GetAllAsync();

        var otherAccounts = currentAccounts
            .Where((record) => record.PublicId != account.PublicId)
            .ToArray();

        await Cache.UpdateAccountsAsync([.. otherAccounts, account]);

        return true;
    }
}