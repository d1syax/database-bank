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
    string Currency
    );