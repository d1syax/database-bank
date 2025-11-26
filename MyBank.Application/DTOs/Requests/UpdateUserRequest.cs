using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record UpdateUserRequest(
    [property: Required] 
    [property: StringLength(50)] 
    string FirstName,
    
    [property: Required] 
    [property: StringLength(50)] 
    string LastName,
    
    [property: Required] 
    [property: Phone]
    string Phone
);