using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Contracts.Interfaces;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Transactions.Commands.UpdateTransactionCache;

public class UpdateTransactionCacheCommand : IRequest
{
    public required CacheUpdateType Type { get; init; }
    public required Guid TransactionId { get; init; }
}

public class TransactionCacheUpdatedMessage : ICacheSubscriptionMessage
{
    public CacheUpdateEntity Entity { get; } = CacheUpdateEntity.Transaction;
    public CacheUpdateType Type { get; init; }
    public Guid Id { get; init; }
}

internal sealed class UpdateTransactionCacheCommandHandler(IUnitOfWork unitOfWork, ICacheUnitOfWork cacheUnitOfWork, ICacheSubscriptionService cacheSubscriptionService) : IRequestHandler<UpdateTransactionCacheCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheUnitOfWork _cacheUnitOfWork = cacheUnitOfWork;
    private readonly ICacheSubscriptionService _cacheSubscriptionService = cacheSubscriptionService;

    public async Task Handle(UpdateTransactionCacheCommand command, CancellationToken cancellationToken)
    {
        var transactions = await _unitOfWork.Transactions.GetAllAsync();
        var categories = await _unitOfWork.TransactionCategories.GetAllAsync();

        var categoryDictionary = categories
            .ToDictionary((kv) => kv.PublicId, (kv) => new CachedTransactionCategory {
                PublicId = kv.PublicId,
                Name = kv.Name,
            });

        var cachedTransactions = transactions
            .Select((transaction) => new CachedTransaction {
                PublicId = transaction.PublicId,
                Description = transaction.Description,
                Value = transaction.Value,
                Date = transaction.Date,
                Category = categoryDictionary[transaction.CategoryId],
            })
            .ToArray();

        await _cacheUnitOfWork
            .Transactions
            .SaveAsync(cachedTransactions);

        await _cacheSubscriptionService
            .Notify(new TransactionCacheUpdatedMessage {
                Type = command.Type,
                Id = command.TransactionId,
            }, cancellationToken);
    }
}