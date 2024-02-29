using KiriathSolutions.Woodstock.Application.Accounts.Commands.CreateAccountCategory;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Web.Requests;
using KiriathSolutions.Woodstock.Web.Types.InputTypes;
using KiriathSolutions.Woodstock.Web.Types.ObjectTypes;
using MediatR;

namespace KiriathSolutions.Woodstock.Web.Types.Resolvers;

internal sealed class AccountCategoryResolvers
{
    private async Task<AccountCategory[]> GetAccountCategories([Service] IUnitOfWork unitOfWork)
    {
        return await unitOfWork
            .AccountCategories
            .GetAllAsync();
    }

    public static void ConfigureQueries(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field("accountCategories")
            .ResolveWith<AccountCategoryResolvers>((resolver) => resolver.GetAccountCategories(default!))
            .Type<ListType<AccountCategoryType>>();
    }

    private Task<AccountCategory> CreateAccountCategory(CreateAccountCategoryRequest input, [Service] ISender sender)
    {
        return sender.Send(new CreateAccountCategoryCommand {
            Name = input.Name,
        });
    }

    public static void ConfigureMutations(IObjectTypeDescriptor<Mutation> descriptor)
    {
        descriptor
            .Field("createAccountCategory")
            .ResolveWith<AccountCategoryResolvers>((resolver) => resolver.CreateAccountCategory(default!, default!))
            .Argument("input", (arg) => arg
                .Type<CreateAccountCategoryInput>()
                .Description("A command containing all the information needed to create an account"))
            .Type<AccountCategoryType>();
    }
}