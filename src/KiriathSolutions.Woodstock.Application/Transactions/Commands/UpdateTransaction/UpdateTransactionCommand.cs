using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommand : IRequest<Transaction>
{
    public required Guid TransactionId { get; init; }
    public required string Description { get; init; }
    public required decimal Value { get; init; }
    public required DateTime Date { get; init; }
}

internal sealed class UpdateTransactionCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionCommand, Transaction>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Transaction> Handle(UpdateTransactionCommand command, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork
            .Transactions
            .GetByPublicIdAsync(command.TransactionId);

        if(transaction is null)
            throw new Exception("Error while saving new Transaction");

        transaction.Value = command.Value;
        transaction.Description = command.Description;
        transaction.Date = command.Date;

        await _unitOfWork
            .Transactions
            .UpdateAsync(transaction);

        _unitOfWork.SaveChanges();

        return transaction;
    }
}