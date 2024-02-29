using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommand : IRequest<bool>
{
    public required Guid TransactionId { get; init; }
}

internal sealed class DeleteTransactionCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTransactionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteTransactionCommand command, CancellationToken cancellationToken)
    {
        var successful = await _unitOfWork
            .Transactions
            .RemoveAsync(command.TransactionId);

        _unitOfWork.SaveChanges();

        return successful;
    }
}