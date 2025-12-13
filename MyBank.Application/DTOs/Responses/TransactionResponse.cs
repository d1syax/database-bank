namespace MyBank.Application.DTOs.Responses;

public record TransactionResponse(
    Guid Id,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string TransactionType,
    string Status,
    string Description,
    DateTime CreatedAt
);