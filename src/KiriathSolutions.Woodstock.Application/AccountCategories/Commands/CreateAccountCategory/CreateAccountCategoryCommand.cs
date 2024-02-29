using KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateAccountCache;
using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Infrastructure.HostedServices;

namespace KiriathSolutions.Woodstock.Application.Accounts.Commands.CreateAccountCategory;

public class CreateAccountCategoryCommand : IRequest<AccountCategory>
{
    public required string Name { get; init; }
}

internal sealed class CreateAccountCategoryCommandHandler(IUnitOfWork unitOfWork, IBackgroundTaskQueue backgroundTaskQueue) : IRequestHandler<CreateAccountCategoryCommand, AccountCategory>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue = backgroundTaskQueue;

    public async Task<AccountCategory> Handle(CreateAccountCategoryCommand command, CancellationToken cancellationToken)
    {
        var publicId = Guid.NewGuid();

        var account = new AccountCategory {
            Id = 0,
            Name = command.Name,
            PublicId = publicId,
        };

        await _unitOfWork
            .AccountCategories
            .AddAsync(account);

        _unitOfWork.SaveChanges();

        var newCategory = await _unitOfWork
            .AccountCategories
            .GetByPublicIdAsync(publicId);

        if(newCategory is null)
            throw new Exception("Error while saving new account");

        await _backgroundTaskQueue
            .QueueAsync(new UpdateAccountCacheCommand {
                Type = CacheUpdateType.Create,
                AccountId = Guid.NewGuid(),
            });

        return newCategory;
    }
}