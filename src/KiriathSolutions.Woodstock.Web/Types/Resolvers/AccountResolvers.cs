using KiriathSolutions.Woodstock.Application.Accounts.Commands.CreateAccount;
using KiriathSolutions.Woodstock.Application.Accounts.Commands.DeleteAccount;
using KiriathSolutions.Woodstock.Application.Accounts.Commands.UpdateAccount;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Web.Requests;
using KiriathSolutions.Woodstock.Web.Types.InputTypes;
using KiriathSolutions.Woodstock.Web.Types.ObjectTypes;
using MediatR;

namespace KiriathSolutions.Woodstock.Web.Types.Resolvers;

internal sealed class AccountResolvers
{
    private static async Task<Account[]> GetAccounts([Service] IUnitOfWork unitOfWork)
    {
        return await unitOfWork
            .Accounts
            .GetAllAsync();
    }

    private static async Task<CachedAccount[]> GetCachedAccounts([Service] ICacheUnitOfWork unitOfWork)
    {
        var accounts = await unitOfWork
            .Accounts
            .GetAllAsync();

        return accounts
            .OrderBy((account) => account.Name)
            .ToArray();
    }

    public static void ConfigureQueries(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field("accounts")
            .UsePaging()
            .ResolveWith<AccountResolvers>((_) => GetCachedAccounts(default!))
            .Type<ListType<CachedAccountType>>();
    }

    private Task<Account> CreateAccount(CreateAccountRequest input, [Service] ISender sender)
    {
        return sender.Send(new CreateAccountCommand {
            Name = input.Name,
            Balance = input.Balance,
            CategoryId = input.CategoryId,
        });
    }

    private Task<Account> UpdateAccount(UpdateAccountRequest input, [Service] ISender sender)
    {
        return sender.Send(new UpdateAccountCommand {
            AccountId = input.Id,
            Name = input.Name,
            Balance = input.Balance,
            CategoryId = input.CategoryId,
        });
    }

    private Task<bool> DeleteAccount(DeleteAccountRequest input, [Service] ISender sender)
    {
        return sender.Send(new DeleteAccountCommand {
            AccountId = input.Id,
        });
    }

    public static void ConfigureMutations(IObjectTypeDescriptor<Mutation> descriptor)
    {
        descriptor
            .Field("createAccount")
            .ResolveWith<AccountResolvers>((resolver) => resolver.CreateAccount(default!, default!))
            .Argument("input", (arg) => arg
                .Type<CreateAccountInput>()
                .Description("A command containing all the information needed to create an account"))
            .Type<AccountType>();

        descriptor
            .Field("updateAccount")
            .ResolveWith<AccountResolvers>((resolver) => resolver.UpdateAccount(default!, default!))
            .Argument("input", (arg) => arg
                .Type<UpdateAccountInput>()
                .Description("A command containing all the information needed to update an account"))
            .Type<AccountType>();

        descriptor
            .Field("deleteAccount")
            .ResolveWith<AccountResolvers>((resolver) => resolver.DeleteAccount(default!, default!))
            .Argument("input", (arg) => arg
                .Type<DeleteAccountInput>()
                .Description("A command containing all the information needed to delete an account"))
            .Type<BooleanType>();
    }
}