using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class LoanEntity : SoftDeletableEntity
{
    private LoanEntity() { }
    private LoanEntity(Guid userId, Guid accountId, decimal principalAmount, decimal interestAmount)
    {
        UserId = userId;
        AccountId = accountId;
        PrincipalAmount = principalAmount;
        InterestAmount = interestAmount;
        PaidAmount = 0;
        Status = LoanStatus.Active;
        IssuedAt = DateTime.Now;
    }
    public Guid UserId { get; private set; }
    public Guid AccountId { get; private set; }
    
    public decimal PrincipalAmount { get; private set; } 
    public decimal InterestAmount { get; private set; }  
    public decimal TotalAmountToRepay => PrincipalAmount + InterestAmount;
    
    public decimal PaidAmount { get; private set; } 
    public LoanStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }

    public UserEntity UserEntity { get; set; } = null!;
    public AccountEntity AccountEntity { get; set; } = null!;
    public ICollection<LoanPaymentEntity> Payments { get; set; } = new List<LoanPaymentEntity>();
    public bool IsFullyPaid => PaidAmount >= TotalAmountToRepay;
    
    public static Result<LoanEntity> Create(Guid userId, Guid accountId, decimal amount, decimal interestRatePercent)
    {
        if (userId == Guid.Empty) return Result.Failure<LoanEntity>("UserId is required");
        if (accountId == Guid.Empty) return Result.Failure<LoanEntity>("AccountId is required");
        
        if (amount <= 0) return Result.Failure<LoanEntity>("LoanEntity amount must be positive");
        if (interestRatePercent < 0) return Result.Failure<LoanEntity>("Interest rate cannot be negative");

        decimal interestAmount = amount * (interestRatePercent / 100m);

        var loan = new LoanEntity(userId, accountId, amount, interestAmount);
        return Result.Success(loan);
    }

    public Result Repay(decimal amount)
    {
        if (Status != LoanStatus.Active)
            return Result.Failure("LoanEntity is not active");

        if (amount <= 0)
            return Result.Failure("Repayment amount must be positive");

        PaidAmount += amount;
        
        if (IsFullyPaid)
        {
            Status = LoanStatus.Paid;
        }

        return Result.Success();
    }
}