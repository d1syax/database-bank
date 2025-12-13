using System.ComponentModel.DataAnnotations;
using MyBank.Domain.Enums;

namespace MyBank.Application.DTOs.Requests;

public record CreateAccountRequest(
    [Required] 
    Guid UserId,
    
    [Required] 
    [StringLength(3, MinimumLength = 3)] 
    [RegularExpression("^(UAH|USD|EUR)$", ErrorMessage = "Currency must be UAH, USD, or EUR")] 
    string Currency,
    
    [Required] 
    AccountType AccountType
);