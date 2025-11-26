using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record RepayLoanRequest(
    [property: Required]
    Guid LoanId,

    [property: Required]
    [property: Range(0.01, 10000000)]
    decimal Amount
);