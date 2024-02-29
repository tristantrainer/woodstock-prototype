using KiriathSolutions.Woodstock.Domain.CachedEntities;

namespace KiriathSolutions.Woodstock.Application.Accounts.Queries.GetAccounts;

public class GetAccountsViewModel
{
    public required CachedAccount[] Accounts { get; init; }
}