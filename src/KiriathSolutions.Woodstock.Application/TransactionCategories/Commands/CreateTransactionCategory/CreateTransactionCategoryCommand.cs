using KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateTransactionCategoryCache;
using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Infrastructure.HostedServices;

namespace KiriathSolutions.Woodstock.Application.Transactions.Commands.CreateTransactionCategory;

public class CreateTransactionCategoryCommand : IRequest<TransactionCategory>
{
    public required string Name { get; init; }
}

internal sealed class CreateTransactionCategoryCommandHandler(IUnitOfWork unitOfWork, IBackgroundTaskQueue backgroundTaskQueue) : IRequestHandler<CreateTransactionCategoryCommand, TransactionCategory>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue = backgroundTaskQueue;

    public async Task<TransactionCategory> Handle(CreateTransactionCategoryCommand command, CancellationToken cancellationToken)
    {
        var publicId = Guid.NewGuid();

        var category = new TransactionCategory {
            Id = 0,
            PublicId = publicId,
            Name = command.Name,
        };

        await _unitOfWork
            .TransactionCategories
            .AddAsync(category);

        _unitOfWork.SaveChanges();

        var newTransaction = await _unitOfWork
            .TransactionCategories
            .GetByPublicIdAsync(publicId);

        if(newTransaction is null)
            throw new Exception("Error while saving new Transaction");

        await _backgroundTaskQueue
            .QueueAsync(new UpdateTransactionCategoryCacheCommand {
                Type = CacheUpdateType.Create,
            });

        return newTransaction;
    }
}