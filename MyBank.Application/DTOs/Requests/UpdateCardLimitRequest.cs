using System.ComponentModel.DataAnnotations;

namespace MyBank.Application.DTOs.Requests;

public record UpdateCardLimitRequest(
    [Required]
    [Range(100, 100000, ErrorMessage = "Limit must be between 100 and 100000")]
    decimal NewDailyLimit
);