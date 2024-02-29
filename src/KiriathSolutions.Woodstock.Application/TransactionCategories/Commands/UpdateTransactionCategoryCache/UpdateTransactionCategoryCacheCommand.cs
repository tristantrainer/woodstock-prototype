using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Contracts.Interfaces;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateTransactionCategoryCache;

public class UpdateTransactionCategoryCacheCommand : IRequest
{
    public required CacheUpdateType Type { get; init; }
}

public class TransactionCategoryCacheUpdatedMessage : ICacheSubscriptionMessage
{
    public CacheUpdateEntity Entity { get; } = CacheUpdateEntity.TransactionCategory;
    public CacheUpdateType Type { get; init; }
    public Guid Id { get; init; }
}

internal sealed class UpdateTransactionCategoryCacheCommandHandler(IUnitOfWork unitOfWork, ICacheUnitOfWork cacheUnitOfWork, ICacheSubscriptionService cacheSubscriptionService) : IRequestHandler<UpdateTransactionCategoryCacheCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheUnitOfWork _cacheUnitOfWork = cacheUnitOfWork;
    private readonly ICacheSubscriptionService _cacheSubscriptionService = cacheSubscriptionService;

    public async Task Handle(UpdateTransactionCategoryCacheCommand command, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.TransactionCategories.GetAllAsync();

        var cachedCategories = categories
            .Select((account) => new CachedTransactionCategory {
                PublicId = account.PublicId,
                Name = account.Name,
            })
            .ToArray();

        await _cacheUnitOfWork
            .TransactionCategories
            .SaveAsync(cachedCategories);

        await _cacheSubscriptionService
            .Notify(new TransactionCategoryCacheUpdatedMessage {
                Type = command.Type,
                Id = Guid.NewGuid(),
            }, cancellationToken);
    }
}