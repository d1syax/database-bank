using System.ComponentModel.DataAnnotations;
using MyBank.Domain.Enums;

public record CreateCardRequest(
    [Required] 
    Guid AccountId,

    [Required] 
    CardType CardType
);