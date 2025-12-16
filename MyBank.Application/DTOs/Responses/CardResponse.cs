namespace MyBank.Application.DTOs.Responses;

public record CardResponse(
    Guid Id,
    string CardNumber,
    string CardType,
    DateTime ExpirationDate,
    string Status,
    decimal DailyLimit
);