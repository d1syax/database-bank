using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record RepayLoanRequest(
    [Required]
    Guid LoanId,

    [Required]
    [Range(0.01, 10000000)]
    decimal Amount
);