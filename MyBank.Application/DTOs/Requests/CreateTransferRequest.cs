using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record CreateTransferRequest(
    [Required] 
    Guid FromAccountId,
    
    [Required] 
    Guid ToAccountId,
    
    [Required] 
    [Range(0.01, 1000000)] 
    decimal Amount,
    
    [StringLength(100)] 
    string Description = "Transfer" 
);