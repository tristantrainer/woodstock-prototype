using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Caches;

namespace KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

public interface ITransactionRepository : IRepository
{
    Task AddAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> RemoveAsync(Guid transactionId);
    Task<Transaction[]> GetAllAsync();
    Task<Transaction?> GetByPublicIdAsync(Guid id);
}

public interface ITransactionCacheRepository : IRepository 
{
    Task SaveAsync(CachedTransaction[] transactions);
    Task AddAsync(CachedTransaction transaction);
    Task<bool> UpdateAsync(CachedTransaction transaction);
    Task<bool> RemoveAsync(Guid transactionId);
    Task<CachedTransaction[]> GetAllAsync();
    Task<CachedTransaction?> GetByIdAsync(Guid id);
}

internal sealed class TransactionsRepository : BaseRepository, ITransactionRepository
{
    public Task AddAsync(Transaction transaction)
    {
        return Task.CompletedTask;
    }

    public Task<Transaction[]> GetAllAsync()
    {
        return Task.FromResult<Transaction[]>([]);
    }

    public Task<Transaction?> GetByPublicIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(Guid transactionId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}

internal sealed class TransactionLiteRepository(IDataCache dataCache) : BaseLiteRepository(dataCache), ITransactionRepository
{
    public async Task AddAsync(Transaction transaction)
    {
        var transactions = await GetAllAsync();

        var lastId = transactions.Length > 0 ? transactions.Max((record) => record.Id) : 0;

        var toSave = new Transaction {
            Description = transaction.Description,
            PublicId = transaction.PublicId,
            Value = transaction.Value,
            Date = transaction.Date,
            CategoryId = transaction.CategoryId,
            Id = lastId + 1,
        };

        await Cache.SaveTransactionsAsync([..transactions, toSave]);
    }

    public Task<Transaction[]> GetAllAsync()
    {
        return Cache.GetTransactionsAsync();
    }

    public async Task<Transaction?> GetByPublicIdAsync(Guid publicId)
    {
        var transactions = await GetAllAsync();

        return transactions.FirstOrDefault((transaction) => transaction.PublicId == publicId);
    }

    public async Task<bool> RemoveAsync(Guid transactionId)
    {
        var currentTransactions = await GetAllAsync();

        var updatedTransactions = currentTransactions
            .Where((record) => record.PublicId != transactionId)
            .ToArray();

        await Cache.SaveTransactionsAsync(updatedTransactions);

        return updatedTransactions.Length != currentTransactions.Length;
    }

    public async Task<bool> UpdateAsync(Transaction transaction)
    {
        var currentTransactions = await GetAllAsync();

        var existingTransaction = currentTransactions
            .FirstOrDefault((record) => record.PublicId == transaction.PublicId);

        if(existingTransaction is null)
            return false;

        var otherTransactions = currentTransactions
            .Where((record) => record.PublicId != transaction.PublicId)
            .ToArray();

        await Cache.SaveTransactionsAsync([.. otherTransactions, existingTransaction with { 
            Description = transaction.Description,
            Value = transaction.Value, 
            Date = transaction.Date, 
        }]);

        return true;
    }
}

internal sealed class TransactionsCacheRepository(IQueryCache queryCache) : BaseCacheRepository(queryCache), ITransactionCacheRepository
{
    public async Task AddAsync(CachedTransaction transaction)
    {
        var transactions = await GetAllAsync();
        await Cache.UpdateTransactionsAsync([..transactions, transaction]);
    }

    public Task<CachedTransaction[]> GetAllAsync()
    {
        return Cache.GetTransactionsAsync();
    }

    public async Task<CachedTransaction?> GetByIdAsync(Guid id)
    {
        var transactions = await GetAllAsync();

        return transactions.FirstOrDefault((transaction) => transaction.PublicId == id);
    }

    public async Task<bool> RemoveAsync(Guid transactionId)
    {
        var currentTransactions = await GetAllAsync();

        var updatedTransactions = currentTransactions
            .Where((record) => record.PublicId != transactionId)
            .ToArray();

        await Cache.UpdateTransactionsAsync(updatedTransactions);

        return updatedTransactions.Length != currentTransactions.Length;
    }

    public async Task SaveAsync(CachedTransaction[] transactions)
    {
        await Cache.UpdateTransactionsAsync(transactions);
    }

    public async Task<bool> UpdateAsync(CachedTransaction transaction)
    {
        var currentTransactions = await GetAllAsync();

        var otherTransactions = currentTransactions
            .Where((record) => record.PublicId != transaction.PublicId)
            .ToArray();

        await Cache.UpdateTransactionsAsync([.. otherTransactions, transaction]);

        return true;
    }
}