using System.ComponentModel.DataAnnotations;

namespace MyBank.Application.DTOs.Requests;

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