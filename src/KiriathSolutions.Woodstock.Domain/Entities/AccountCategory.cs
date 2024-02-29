namespace KiriathSolutions.Woodstock.Domain.Entities;

public sealed record AccountCategory
{
    public required int Id { get; init; }
    public required Guid PublicId { get; init; }
    public required string Name { get; init; }
}
