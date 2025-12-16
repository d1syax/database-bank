using System.ComponentModel.DataAnnotations;

namespace MyBank.Application.DTOs.Requests;

public record CreateLoanRequest(
    [Required] 
    Guid UserId,

    [Required] 
    Guid AccountId, 

    [Required] 
    [Range(100, 10000000)] 
    decimal Amount,

    [Required] 
    [Range(0.1, 100)] 
    decimal InterestRate 
);