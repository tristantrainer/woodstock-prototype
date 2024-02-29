using KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateAccountCache;
using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Infrastructure.HostedServices;

namespace KiriathSolutions.Woodstock.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommand : IRequest<Account>
{
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
    public required Guid CategoryId { get; init; }
}

internal sealed class CreateAccountCommandHandler(IUnitOfWork unitOfWork, IBackgroundTaskQueue backgroundTaskQueue) : IRequestHandler<CreateAccountCommand, Account>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue = backgroundTaskQueue;

    public async Task<Account> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var publicId = Guid.NewGuid();

        var account = new Account {
            Id = 0,
            Name = command.Name,
            Balance = command.Balance,
            CategoryId = command.CategoryId,
            PublicId = publicId,
        };

        await _unitOfWork
            .Accounts
            .AddAsync(account);

        _unitOfWork.SaveChanges();

        var newAccount = await _unitOfWork
            .Accounts
            .GetByPublicIdAsync(publicId);

        if(newAccount is null)
            throw new Exception("Error while saving new account");

        await _backgroundTaskQueue
            .QueueAsync(new UpdateAccountCacheCommand {
                Type = CacheUpdateType.Create,
                AccountId = publicId,
            });

        return newAccount;
    }
}