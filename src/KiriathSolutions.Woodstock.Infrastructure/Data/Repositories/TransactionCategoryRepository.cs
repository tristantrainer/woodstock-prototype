using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface ITransactionCategoryRepository : IRepository
{
    Task AddAsync(TransactionCategory transaction);
    Task<bool> UpdateAsync(TransactionCategory transaction);
    Task<bool> RemoveAsync(Guid transactionId);
    Task<TransactionCategory[]> GetAllAsync();
    Task<TransactionCategory?> GetByPublicIdAsync(Guid id);
}

public interface ITransactionCategoryCacheRepository : IRepository 
{
    Task SaveAsync(CachedTransactionCategory[] transactions);
    Task AddAsync(CachedTransactionCategory transaction);
    Task<bool> UpdateAsync(CachedTransactionCategory transaction);
    Task<bool> RemoveAsync(Guid transactionId);
    Task<CachedTransactionCategory[]> GetAllAsync();
    Task<CachedTransactionCategory?> GetByIdAsync(Guid id);
}

internal sealed class TransactionCategoryRepository : BaseRepository, ITransactionCategoryRepository
{
    public Task AddAsync(TransactionCategory transaction)
    {
        return Task.CompletedTask;
    }

    public Task<TransactionCategory[]> GetAllAsync()
    {
        return Task.FromResult<TransactionCategory[]>([]);
    }

    public Task<TransactionCategory?> GetByPublicIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(Guid transactionId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(TransactionCategory transaction)
    {
        throw new NotImplementedException();
    }
}

internal sealed class TransactionCategoryLiteRepository(IDataCache dataCache) : BaseLiteRepository(dataCache), ITransactionCategoryRepository
{
    public async Task AddAsync(TransactionCategory transaction)
    {
        var categories = await GetAllAsync();

        var lastId = categories.Length > 0 ? categories.Max((record) => record.Id) : 0;

        var toSave = new TransactionCategory {
            PublicId = transaction.PublicId,
            Name = transaction.Name,
            Id = lastId + 1,
        };

        await Cache.SaveTransactionCategoriesAsync([..categories, toSave]);
    }

    public Task<TransactionCategory[]> GetAllAsync()
    {
        return Cache.GetTransactionCategoriesAsync();
    }

    public async Task<TransactionCategory?> GetByPublicIdAsync(Guid publicId)
    {
        var transactions = await GetAllAsync();

        return transactions.FirstOrDefault((category) => category.PublicId == publicId);
    }

    public async Task<bool> RemoveAsync(Guid categoryId)
    {
        var currentCategories = await GetAllAsync();

        var updatedCategories = currentCategories
            .Where((record) => record.PublicId != categoryId)
            .ToArray();

        await Cache.SaveTransactionCategoriesAsync(updatedCategories);

        return updatedCategories.Length != currentCategories.Length;
    }

    public async Task<bool> UpdateAsync(TransactionCategory category)
    {
        var currentCategories = await GetAllAsync();

        var existingCategory = currentCategories
            .FirstOrDefault((record) => record.PublicId == category.PublicId);

        if(existingCategory is null)
            return false;

        var otherCategories = currentCategories
            .Where((record) => record.PublicId != category.PublicId)
            .ToArray();

        await Cache.SaveTransactionCategoriesAsync([.. otherCategories, existingCategory with { 
            Name = category.Name,
        }]);

        return true;
    }
}

internal sealed class TransactionCategoryCacheRepository(IQueryCache queryCache) : BaseCacheRepository(queryCache), ITransactionCategoryCacheRepository
{
    public async Task AddAsync(CachedTransactionCategory category)
    {
        var categories = await GetAllAsync();
        await Cache.UpdateTransactionCategoriesAsync([..categories, category]);
    }

    public Task<CachedTransactionCategory[]> GetAllAsync()
    {
        return Cache.GetTransactionCategoriesAsync();
    }

    public async Task<CachedTransactionCategory?> GetByIdAsync(Guid id)
    {
        var transactions = await GetAllAsync();

        return transactions.FirstOrDefault((category) => category.PublicId == id);
    }

    public async Task<bool> RemoveAsync(Guid transactionId)
    {
        var currentCategories = await GetAllAsync();

        var updateCategories = currentCategories
            .Where((record) => record.PublicId != transactionId)
            .ToArray();

        await Cache.UpdateTransactionCategoriesAsync(updateCategories);

        return updateCategories.Length != currentCategories.Length;
    }

    public async Task SaveAsync(CachedTransactionCategory[] categories)
    {
        await Cache.UpdateTransactionCategoriesAsync(categories);
    }

    public async Task<bool> UpdateAsync(CachedTransactionCategory category)
    {
        var currentTransactions = await GetAllAsync();

        var otherCategories = currentTransactions
            .Where((record) => record.PublicId != category.PublicId)
            .ToArray();

        await Cache.UpdateTransactionCategoriesAsync([.. otherCategories, category]);

        return true;
    }
}