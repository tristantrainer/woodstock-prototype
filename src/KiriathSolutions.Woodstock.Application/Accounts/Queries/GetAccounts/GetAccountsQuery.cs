using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;

namespace KiriathSolutions.Woodstock.Application.Accounts.Queries.GetAccounts;

public sealed class GetAccountsQuery : IRequest<GetAccountsViewModel>
{

}

internal sealed class GetPotentialCrossoversQueryHandler(ICacheUnitOfWork unitOfWork) : IRequestHandler<GetAccountsQuery, GetAccountsViewModel>
{
    private readonly ICacheUnitOfWork _unitOfWork = unitOfWork;

    public async Task<GetAccountsViewModel> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _unitOfWork.Accounts.GetAllAsync();

        return new GetAccountsViewModel {
            Accounts = accounts
        };
    }
}