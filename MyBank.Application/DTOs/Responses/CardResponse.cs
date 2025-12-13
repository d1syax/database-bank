namespace MyBank.Api.DTOs.Responses;

public record CardResponse(
    Guid Id,
    string CardNumber,
    string CardType,
    DateTime ExpirationDate,
    string Status,
    decimal DailyLimit
);