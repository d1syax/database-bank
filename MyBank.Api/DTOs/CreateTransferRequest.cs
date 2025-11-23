using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record CreateTransferRequest(
    [property: Required] 
    Guid FromAccountId,
    
    [property: Required] 
    Guid ToAccountId,
    
    [property: Required] 
    [property: Range(0.01, 1000000)] 
    decimal Amount,
    
    [property: StringLength(100)] 
    string Description = "Transfer" 
);