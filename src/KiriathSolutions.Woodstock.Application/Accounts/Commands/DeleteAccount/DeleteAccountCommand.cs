using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommand : IRequest<bool>
{
    public required Guid AccountId { get; init; }
}

internal sealed class DeleteAccountCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var successful = await _unitOfWork
            .Accounts
            .RemoveAsync(command.AccountId);

        _unitOfWork.SaveChanges();

        return successful;
    }
}