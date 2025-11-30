using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record OpenDepositRequest(
    [Required]
    Guid UserId,
    [Required]
    Guid FromAccountId,
    [Required]
    [Range(0, 1000000)]
    decimal Amount,
    [Required]
    string Currency
    );