using KiriathSolutions.Woodstock.Application.Transactions.Commands.UpdateTransactionCache;
using KiriathSolutions.Woodstock.Contracts.Constants;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Infrastructure.HostedServices;

namespace KiriathSolutions.Woodstock.Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommand : IRequest<Transaction>
{
    public required string Description { get; init; }
    public required decimal Value { get; init; }
    public required DateTime Date { get; init; }
    public required Guid CategoryId { get; init; }
}

internal sealed class CreateTransactionCommandHandler(IUnitOfWork unitOfWork, IBackgroundTaskQueue backgroundTaskQueue) : IRequestHandler<CreateTransactionCommand, Transaction>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue = backgroundTaskQueue;

    public async Task<Transaction> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        var publicId = Guid.NewGuid();

        var transaction = new Transaction {
            Id = 0,
            Description = command.Description,
            Value = command.Value,
            Date = command.Date,
            PublicId = publicId,
            CategoryId = command.CategoryId,
        };

        await _unitOfWork
            .Transactions
            .AddAsync(transaction);

        _unitOfWork.SaveChanges();

        var newTransaction = await _unitOfWork
            .Transactions
            .GetByPublicIdAsync(publicId);

        if(newTransaction is null)
            throw new Exception("Error while saving new Transaction");

        await _backgroundTaskQueue
            .QueueAsync(new UpdateTransactionCacheCommand {
                Type = CacheUpdateType.Create,
                TransactionId = publicId,
            });

        return newTransaction;
    }
}