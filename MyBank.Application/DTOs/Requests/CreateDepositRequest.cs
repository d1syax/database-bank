using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record DepositRequest(
    [Required] 
    [Range(0.01, 10000000)] 
    decimal Amount
);