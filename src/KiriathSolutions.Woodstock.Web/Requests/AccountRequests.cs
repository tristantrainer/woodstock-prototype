namespace KiriathSolutions.Woodstock.Web.Requests;

public record CreateAccountRequest(string Name, decimal Balance, Guid CategoryId);
public record UpdateAccountRequest(Guid Id, string Name, decimal Balance, Guid CategoryId);
public record DeleteAccountRequest(Guid Id);
