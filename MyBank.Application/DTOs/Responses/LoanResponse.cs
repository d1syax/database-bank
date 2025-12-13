namespace MyBank.Application.DTOs.Responses;

public record LoanResponse(
    Guid Id,
    decimal PrincipalAmount,
    decimal InterestAmount,
    decimal PaidAmount,
    decimal TotalAmountToRepay,
    string Status,
    DateTime IssuedAt
);