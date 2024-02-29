using KiriathSolutions.Woodstock.Domain.CachedEntities;

namespace KiriathSolutions.Woodstock.Web.Types.ObjectTypes;

public class CachedTransactionType : ObjectType<CachedTransaction>
{
    protected override void Configure(IObjectTypeDescriptor<CachedTransaction> descriptor)
    {
        base.Configure(descriptor);

        descriptor
            .Field((transaction) => transaction.PublicId)
            .Name("id");
    }
}
