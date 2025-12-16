using System.ComponentModel.DataAnnotations;
using MyBank.Domain.Enums;

namespace MyBank.Application.DTOs.Requests;

public record CreateCardRequest(
    [Required] 
    Guid AccountId,

    [Required] 
    CardType CardType
);