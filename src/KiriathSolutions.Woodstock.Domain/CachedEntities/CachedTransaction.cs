namespace KiriathSolutions.Woodstock.Domain.CachedEntities;

public sealed class CachedTransaction
{
    public required Guid PublicId { get; init; }
    public required string Description { get; init; }
    public required decimal Value { get; init; }
    public required DateTime Date { get; init; }
    public required CachedTransactionCategory Category { get; init; }
}