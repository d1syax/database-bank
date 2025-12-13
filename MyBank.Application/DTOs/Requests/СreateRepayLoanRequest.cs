using System.ComponentModel.DataAnnotations;

namespace MyBank.Application.DTOs.Requests;

public record RepayLoanRequest(
    [Required]
    Guid LoanId,

    [Required]
    [Range(0.01, 10000000)]
    decimal Amount
);