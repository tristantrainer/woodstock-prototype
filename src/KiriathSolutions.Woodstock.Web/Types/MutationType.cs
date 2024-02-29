using KiriathSolutions.Woodstock.Web.Types.Resolvers;

namespace KiriathSolutions.Woodstock.Web.Types;

public class Mutation { }

public class MutationType : ObjectType<Mutation>
{
    protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
    {
        AccountResolvers.ConfigureMutations(descriptor);
        AccountCategoryResolvers.ConfigureMutations(descriptor);
        TransactionResolvers.ConfigureMutations(descriptor);
        TransactionCategoryResolvers.ConfigureMutations(descriptor);
    }
}