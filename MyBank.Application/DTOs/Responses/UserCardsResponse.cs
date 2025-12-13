namespace MyBank.Api.DTOs.Responses;

public record UserCardsResponse(
    Guid CardId,
    string CardNumber,
    string CardType,
    string AccountNumber,
    decimal AccountBalance,
    string Status,
    decimal DailyLimit
);