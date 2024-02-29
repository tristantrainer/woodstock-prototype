using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Contracts.Interfaces;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateAccountCache;

public class UpdateAccountCacheCommand : IRequest
{
    public required CacheUpdateType Type { get; init; }
    public required Guid AccountId { get; init; }
}

public class AccountCacheUpdatedMessage : ICacheSubscriptionMessage
{
    public CacheUpdateEntity Entity { get; } = CacheUpdateEntity.Account;
    public CacheUpdateType Type { get; init; }
    public Guid Id { get; init; }
}

internal sealed class UpdateAccountCacheCommandHandler(IUnitOfWork unitOfWork, ICacheUnitOfWork cacheUnitOfWork, ICacheSubscriptionService cacheSubscriptionService) : IRequestHandler<UpdateAccountCacheCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheUnitOfWork _cacheUnitOfWork = cacheUnitOfWork;
    private readonly ICacheSubscriptionService _cacheSubscriptionService = cacheSubscriptionService;

    public async Task Handle(UpdateAccountCacheCommand command, CancellationToken cancellationToken)
    {
        var accounts = await _unitOfWork.Accounts.GetAllAsync();
        var categories = await _unitOfWork.AccountCategories.GetAllAsync();

        var categoryDictionary = categories
            .ToDictionary((kv) => kv.PublicId, (kv) => new CachedAccountCategory {
                Id = kv.PublicId,
                Name = kv.Name,
            });

        var cachedAccounts = accounts
            .Select((account) => new CachedAccount {
                PublicId = account.PublicId,
                Name = account.Name,
                Category = categoryDictionary[account.CategoryId],
                Balance = account.Balance,
            })
            .ToArray();

        await _cacheUnitOfWork
            .Accounts
            .SaveAsync(cachedAccounts);

        await _cacheSubscriptionService
            .Notify(new AccountCacheUpdatedMessage {
                Type = command.Type,
                Id = command.AccountId,
            });
    }
}