using System.ComponentModel.DataAnnotations;

namespace MyBank.Api.DTOs;

public record UpdateCardLimitRequest(
    [Required]
    [Range(100, 100000, ErrorMessage = "Limit must be between 100 and 100000")]
    decimal NewDailyLimit
);