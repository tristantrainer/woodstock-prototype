using KiriathSolutions.Woodstock.Web.Types.Resolvers;

namespace KiriathSolutions.Woodstock.Web.Types;

internal sealed class Query {}

internal sealed class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        AccountResolvers.ConfigureQueries(descriptor);
        AccountCategoryResolvers.ConfigureQueries(descriptor);
        TransactionResolvers.ConfigureQueries(descriptor);
        TransactionCategoryResolvers.ConfigureQueries(descriptor);
    }
}