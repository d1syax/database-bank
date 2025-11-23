using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record CreateLoanRequest(
    [property: Required] 
    Guid UserId,

    [property: Required] 
    Guid AccountId, 

    [property: Required] 
    [property: Range(100, 10000000)] 
    decimal Amount,

    [property: Required] 
    [property: Range(0.1, 100)] 
    decimal InterestRate 
);