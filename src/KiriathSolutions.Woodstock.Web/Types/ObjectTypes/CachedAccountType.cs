using KiriathSolutions.Woodstock.Domain.CachedEntities;

namespace KiriathSolutions.Woodstock.Web.Types.ObjectTypes;

public class CachedAccountType : ObjectType<CachedAccount>
{
    protected override void Configure(IObjectTypeDescriptor<CachedAccount> descriptor)
    {
        base.Configure(descriptor);

        descriptor
            .Field((transaction) => transaction.PublicId)
            .Name("id");
    }
}
