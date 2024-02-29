using KiriathSolutions.Woodstock.Application.Transactions.Commands.CreateTransaction;
using KiriathSolutions.Woodstock.Application.Transactions.Commands.DeleteTransaction;
using KiriathSolutions.Woodstock.Application.Transactions.Commands.UpdateTransaction;
using KiriathSolutions.Woodstock.Domain.CachedEntities;
using KiriathSolutions.Woodstock.Domain.Entities;
using KiriathSolutions.Woodstock.Infrastructure.Data.Repositories;
using KiriathSolutions.Woodstock.Web.Requests;
using KiriathSolutions.Woodstock.Web.Types.InputTypes;
using KiriathSolutions.Woodstock.Web.Types.ObjectTypes;
using MediatR;

namespace KiriathSolutions.Woodstock.Web.Types.Resolvers;

internal sealed class TransactionResolvers
{
    private static async Task<Transaction[]> GetTransactions([Service] IUnitOfWork unitOfWork)
    {
        return await unitOfWork
            .Transactions
            .GetAllAsync();
    }

    private static async Task<CachedTransaction[]> GetCachedTransactions([Service] ICacheUnitOfWork unitOfWork)
    {
        var transactions = await unitOfWork
            .Transactions
            .GetAllAsync();

        return [.. transactions.OrderByDescending((transaction) => transaction.Description)];
    }

    public static void ConfigureQueries(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field("transactions")
            .UsePaging()
            .UseFiltering()
            .ResolveWith<TransactionResolvers>((_) => GetCachedTransactions(default!))
            .Type<ListType<CachedTransactionType>>();
    }

    private Task<Transaction> CreateTransaction(CreateTransactionRequest input, [Service] ISender sender)
    {
        return sender.Send(new CreateTransactionCommand {
            CategoryId = input.CategoryId,
            Description = input.Description,
            Value = input.Value,
            Date = input.Date,
        });
    }

    private Task<Transaction> UpdateTransaction(UpdateTransactionRequest input, [Service] ISender sender)
    {
        return sender.Send(new UpdateTransactionCommand {
            TransactionId = input.Id,
            Description = input.Description,
            Value = input.Value,
            Date = input.Date,
        });
    }

    private Task<bool> DeleteTransaction(DeleteTransactionRequest input, [Service] ISender sender)
    {
        return sender.Send(new DeleteTransactionCommand {
            TransactionId = input.Id,
        });
    }

    public static void ConfigureMutations(IObjectTypeDescriptor<Mutation> descriptor)
    {
        descriptor
            .Field("createTransaction")
            .ResolveWith<TransactionResolvers>((resolver) => resolver.CreateTransaction(default!, default!))
            .Argument("input", (arg) => arg
                .Type<CreateTransactionInput>()
                .Description("A command containing all the information needed to create an Transaction"))
            .Type<TransactionType>();

        descriptor
            .Field("updateTransaction")
            .ResolveWith<TransactionResolvers>((resolver) => resolver.UpdateTransaction(default!, default!))
            .Argument("input", (arg) => arg
                .Type<UpdateTransactionInput>()
                .Description("A command containing all the information needed to update an Transaction"))
            .Type<TransactionType>();

        descriptor
            .Field("deleteTransaction")
            .ResolveWith<TransactionResolvers>((resolver) => resolver.DeleteTransaction(default!, default!))
            .Argument("input", (arg) => arg
                .Type<DeleteTransactionInput>()
                .Description("A command containing all the information needed to delete an Transaction"))
            .Type<BooleanType>();
    }
}