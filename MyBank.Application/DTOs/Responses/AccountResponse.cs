namespace MyBank.Application.DTOs.Responses;

public record AccountResponse(
    Guid Id,
    Guid UserId,
    string AccountNumber,
    decimal Balance,
    string Currency,
    string AccountType, 
    string Status,
    DateTime OpenedAt
);