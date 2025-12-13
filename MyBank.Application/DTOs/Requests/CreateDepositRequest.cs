using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record CreateDepositRequest(
    [Required]
    Guid UserId,
    [Required]
    Guid AccountId,
    [Required]
    [Range(0, 1000000)]
    decimal Amount,
    [Required]
    [RegularExpression("^(UAH|USD|EUR)$", ErrorMessage = "Currency must be UAH, USD, or EUR")]
    string Currency
    );