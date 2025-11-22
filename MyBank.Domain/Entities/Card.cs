using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class Card : SoftDeletableEntity
{
    private Card() { }
    private Card(Guid accountId, CardType cardType, string cardNumber, string cvv, DateTime expirationDate)
    {
        AccountId = accountId;
        CardType = cardType;
        CardNumber = cardNumber;
        CVV = cvv;
        ExpirationDate = expirationDate;
        Status = CardStatus.Active;
        DailyLimit = 10000;
    }
    
    public Guid AccountId { get; private set; } 
    public string CardNumber { get; private set; } = string.Empty;
    public CardType CardType { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public string CVV { get; private set; } = string.Empty; 
    public CardStatus Status { get; private set; }
    public decimal DailyLimit { get; private set; }
    public DateTime? BlockedAt { get; private set; }

    public Account Account { get; set; } = null!;

    public bool IsActive => Status == CardStatus.Active && !IsExpired;
    public bool IsExpired => ExpirationDate < DateTime.Now; 
    
    public string MaskedCardNumber => $"****-****-****-{CardNumber.Substring(Math.Max(0, CardNumber.Length - 4))}";
    
    public static Result<Card> Create(Guid accountId, CardType cardType)
    {
        if (accountId == Guid.Empty)
            return Result.Failure<Card>("AccountId is required");
        
        var rnd = new Random();

        string prefix = cardType == CardType.Credit ? "5" : "4";
        string number = $"{prefix}{rnd.Next(100, 999)}{rnd.Next(1000, 9999)}{rnd.Next(1000, 9999)}{rnd.Next(1000, 9999)}";

        string cvv = rnd.Next(100, 999).ToString();

        DateTime expiration = DateTime.UtcNow.AddYears(4);

        var card = new Card(accountId, cardType, number, cvv, expiration);

        return Result.Success(card);
    }
    
    public Result Block()
    {
        if (Status == CardStatus.Blocked)
            return Result.Failure("Card is already blocked");

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