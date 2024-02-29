namespace KiriathSolutions.Woodstock.Web.Requests;

public record CreateTransactionRequest(string Description, Guid CategoryId, DateTime Date, decimal Value);
public record UpdateTransactionRequest(Guid Id, string Description, DateTime Date, decimal Value);
public record DeleteTransactionRequest(Guid Id);
