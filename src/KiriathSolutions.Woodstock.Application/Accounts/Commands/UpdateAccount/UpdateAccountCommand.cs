using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommand : IRequest<Account>
{
    public required Guid AccountId { get; init; }
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
    public required Guid CategoryId { get; init; }
}

internal sealed class UpdateAccountCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateAccountCommand, Account>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Account> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts
            .GetByPublicIdAsync(command.AccountId);

        if(account is null)
            throw new Exception("Error while saving new account");

        account.Balance = command.Balance;
        account.Name = command.Name;
        account.CategoryId = command.CategoryId;

        await _unitOfWork
            .Accounts
            .UpdateAsync(account);

        _unitOfWork.SaveChanges();

        return account;
    }
}