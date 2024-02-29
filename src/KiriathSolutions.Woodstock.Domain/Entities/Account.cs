namespace KiriathSolutions.Woodstock.Domain.Entities;

public sealed record Account
{
    public required int Id { get; init; }
    public required Guid PublicId { get; init; }
    public required string Name { get; set; }
    public required decimal Balance { get; set; }
    public required Guid CategoryId { get; set; }
}