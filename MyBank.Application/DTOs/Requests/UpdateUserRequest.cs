using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record UpdateUserRequest(
    [Required] 
    [StringLength(50)] 
    string FirstName,
    
    [Required] 
    [StringLength(50)] 
    string LastName,
    
    [Required] 
    [Phone]
    string Phone
);