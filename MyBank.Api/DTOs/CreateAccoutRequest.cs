using System.ComponentModel.DataAnnotations;
using DefaultNamespace;

namespace MyBank.Api.DTOs;

public record CreateAccountRequest(
    [property: Required] 
    Guid UserId,
    
    [property: Required] 
    [property: StringLength(3, MinimumLength = 3)] 
    string Currency,
    
    [property: Required] 
    AccountType AccountType
);