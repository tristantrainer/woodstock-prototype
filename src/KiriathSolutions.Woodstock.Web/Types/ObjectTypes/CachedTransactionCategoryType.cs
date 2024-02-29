using KiriathSolutions.Woodstock.Domain.CachedEntities;

namespace KiriathSolutions.Woodstock.Web.Types.ObjectTypes;

public class CachedTransactionCategoryType : ObjectType<CachedTransactionCategory>
{
    protected override void Configure(IObjectTypeDescriptor<CachedTransactionCategory> descriptor)
    {
        base.Configure(descriptor);

        descriptor
            .Field((transaction) => transaction.PublicId)
            .Name("id");
    }
}
