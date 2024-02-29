using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface IAccountCategoriesRepository : IRepository
{
    Task AddAsync(AccountCategory category);
    Task<bool> UpdateAsync(AccountCategory category);
    Task<bool> RemoveAsync(Guid categoryId);
    Task<AccountCategory[]> GetAllAsync();
    Task<AccountCategory?> GetByPublicIdAsync(Guid publicId);
}

internal sealed class AccountCategoriesRepository : BaseRepository, IAccountCategoriesRepository
{
    public Task AddAsync(AccountCategory category)
    {
        throw new NotImplementedException();
    }

    public Task<AccountCategory[]> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AccountCategory?> GetByPublicIdAsync(Guid publicId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(Guid categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(AccountCategory category)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AccountCategoriesLiteRepository(IDataCache dataCache) : BaseLiteRepository(dataCache), IAccountCategoriesRepository
{
    public async Task AddAsync(AccountCategory account)
    {
        var categories = await GetAllAsync();

        var lastId = categories.Length > 0 ? categories.Max((record) => record.Id) : 0;

        var toSave = new AccountCategory {
            Name = account.Name,
            PublicId = account.PublicId,
            Id = lastId + 1,
        };

        await Cache.SaveAccountCategoriesAsync([..categories, toSave]);
    }

    public Task<AccountCategory[]> GetAllAsync()
    {
        return Cache.GetAccountCategoriesAsync();
    }

    public async Task<AccountCategory?> GetByPublicIdAsync(Guid publicId)
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

        await Cache.SaveAccountCategoriesAsync(updatedAccounts);

        return updatedAccounts.Length != currentAccounts.Length;
    }

    public async Task<bool> UpdateAsync(AccountCategory account)
    {
        var currentAccounts = await GetAllAsync();

        var existingAccount = currentAccounts
            .FirstOrDefault((record) => record.PublicId == account.PublicId);

        if(existingAccount is null)
            return false;

        var otherAccounts = currentAccounts
            .Where((record) => record.PublicId != account.PublicId)
            .ToArray();

        await Cache.SaveAccountCategoriesAsync([.. otherAccounts, existingAccount with { Name = account.Name }]);

        return true;
    }
}