using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record CreateUserRequest(
    [Required] 
    [StringLength(50)] 
    string FirstName,
    
    [Required] 
    [StringLength(50)] 
    string LastName,
    
    [Required] 
    [EmailAddress] 
    string Email,
    
    [Required] 
    [Phone]
    string Phone,
    
    [Required] 
    string Password,
    
    [Required] 
    DateTime DateOfBirth
);