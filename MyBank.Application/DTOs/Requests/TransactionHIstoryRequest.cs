namespace MyBank.Application.DTOs.Requests;
using System.ComponentModel.DataAnnotations;

public record TransactionHistoryRequest
(
    [Required]
    Guid AccountId,
    
    [Required]
    DateTime StartDate,
    
    [Required]
    DateTime EndDate
);
