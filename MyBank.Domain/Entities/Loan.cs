using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class Loan : SoftDeletableEntity
{
    private Loan() { }
    private Loan(Guid userId, Guid accountId, decimal principalAmount, decimal interestAmount)
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

    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();
    public bool IsFullyPaid => PaidAmount >= TotalAmountToRepay;
    
    public static Result<Loan> Create(Guid userId, Guid accountId, decimal amount, decimal interestRatePercent)
    {
        if (userId == Guid.Empty) return Result.Failure<Loan>("UserId is required");
        if (accountId == Guid.Empty) return Result.Failure<Loan>("AccountId is required");
        
        if (amount <= 0) return Result.Failure<Loan>("Loan amount must be positive");
        if (interestRatePercent < 0) return Result.Failure<Loan>("Interest rate cannot be negative");

        decimal interestAmount = amount * (interestRatePercent / 100m);

        var loan = new Loan(userId, accountId, amount, interestAmount);
        return Result.Success(loan);
    }

    public Result Repay(decimal amount)
    {
        if (Status != LoanStatus.Active)
            return Result.Failure("Loan is not active");

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