using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record CreateUserRequest(
    [property: Required] 
    [property: StringLength(50)] 
    string FirstName,
    
    [property: Required] 
    [property: StringLength(50)] 
    string LastName,
    
    [property: Required] 
    [property: EmailAddress] 
    string Email,
    
    [property: Required] 
    [property: Phone]
    string Phone,
    
    [property: Required] 
    string Password,
    
    [property: Required] 
    DateTime DateOfBirth
);