namespace KiriathSolutions.Woodstock.Domain.Entities;

public sealed record Transaction
{
    public required int Id { get; init; }
    public required Guid PublicId { get; init; }
    public required string Description { get; set; }
    public required DateTime Date { get; set; }
    public required decimal Value { get; set; }
    public required Guid CategoryId { get; set; }
}
