using System.ComponentModel.DataAnnotations;
using DefaultNamespace;

public record CreateCardRequest(
    [property: Required] 
    Guid AccountId,

    [property: Required] 
    CardType CardType
);