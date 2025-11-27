using CSharpFunctionalExtensions;
using MyBank.Domain.Common; 
using MyBank.Domain.Enums;  

namespace MyBank.Domain.Entities;

public class CardEntity : SoftDeletableEntity
{
    private CardEntity() { }
    private CardEntity(Guid accountId, CardType cardType, string cardNumber, DateTime expirationDate)
    {
        AccountId = accountId;
        CardType = cardType;
        CardNumber = cardNumber;
        ExpirationDate = expirationDate;
        Status = CardStatus.Active;
        DailyLimit = 10000;
    }
    
    public Guid AccountId { get; private set; } 
    public string CardNumber { get; private set; } = string.Empty;
    public CardType CardType { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public CardStatus Status { get; private set; }
    public decimal DailyLimit { get; private set; }
    public DateTime? BlockedAt { get; private set; }

    public AccountEntity AccountEntity { get; set; } = null!;

    public bool IsActive => Status == CardStatus.Active && !IsExpired;
    public bool IsExpired => ExpirationDate < DateTime.UtcNow; 
    
    public string MaskedCardNumber => $"****-****-****-{CardNumber.Substring(Math.Max(0, CardNumber.Length - 4))}";
    
    public static Result<CardEntity> Create(Guid accountId, CardType cardType)
    {
        if (accountId == Guid.Empty)
            return Result.Failure<CardEntity>("AccountId is required");
        

        var guidPortion = Guid.NewGuid().ToString("N")[..15];
        var prefix = cardType == CardType.Credit ? "5" : "4";
        var number = $"{prefix}{guidPortion}";
        DateTime expiration = DateTime.UtcNow.AddYears(4);

        var card = new CardEntity(accountId, cardType, number, expiration);
        return Result.Success(card);
    }
    
    public Result Block()
    {
        if (Status == CardStatus.Blocked)
            return Result.Failure("CardEntity is already blocked");

        Status = CardStatus.Blocked;
        BlockedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result ChangeLimit(decimal newLimit)
    {
        if (newLimit < 0)
            return Result.Failure("Limit cannot be negative");
            
        DailyLimit = newLimit;
        return Result.Success();
    }
}