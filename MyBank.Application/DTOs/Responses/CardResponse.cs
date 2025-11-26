namespace MyBank.Api.DTOs.Responses;

public record CardResponse(
    Guid Id,
    string MaskedCardNumber,
    string CardType,
    DateTime ExpirationDate,
    string Status,
    decimal DailyLimit
);