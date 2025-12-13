namespace MyBank.Api.DTOs.Responses;

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string DateOfBirth,
    DateTime CreatedAt
);