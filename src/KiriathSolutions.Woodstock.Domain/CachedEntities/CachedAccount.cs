namespace KiriathSolutions.Woodstock.Domain.CachedEntities;

public sealed class CachedAccount
{
    public required Guid PublicId { get; init; }
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
    public required CachedAccountCategory Category { get; init; }
}

public sealed class CachedAccountCategory 
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
