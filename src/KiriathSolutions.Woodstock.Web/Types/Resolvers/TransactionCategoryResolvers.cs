using KiriathSolutions.Woodstock.Application.Transactions.Commands.CreateTransactionCategory;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Web.Requests;
using KiriathSolutions.Woodstock.Web.Types.InputTypes;
using KiriathSolutions.Woodstock.Web.Types.ObjectTypes;
using MediatR;

namespace KiriathSolutions.Woodstock.Web.Types.Resolvers;

internal sealed class TransactionCategoryResolvers
{
    private static async Task<CachedTransactionCategory[]> GetCachedTransactionCategories([Service] ICacheUnitOfWork unitOfWork)
    {
        var transactions = await unitOfWork
            .TransactionCategories
            .GetAllAsync();

        return [.. transactions.OrderByDescending((transaction) => transaction.Name)];
    }

    public static void ConfigureQueries(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field("transactionCategories")
            .ResolveWith<TransactionCategoryResolvers>((_) => GetCachedTransactionCategories(default!))
            .Type<ListType<CachedTransactionCategoryType>>();
    }

    private Task<TransactionCategory> CreateTransactionCategory(CreateTransactionCategoryRequest input, [Service] ISender sender)
    {
        return sender.Send(new CreateTransactionCategoryCommand {
            Name = input.Name,
        });
    }

    public static void ConfigureMutations(IObjectTypeDescriptor<Mutation> descriptor)
    {
        descriptor
            .Field("createTransactionCategory")
            .ResolveWith<TransactionCategoryResolvers>((resolver) => resolver.CreateTransactionCategory(default!, default!))
            .Argument("input", (arg) => arg
                .Type<CreateTransactionCategoryInput>()
                .Description("A command containing all the information needed to create an Transaction"))
            .Type<TransactionCategoryType>();
    }
}
