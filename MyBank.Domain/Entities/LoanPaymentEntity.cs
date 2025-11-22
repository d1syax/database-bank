using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class LoanPaymentEntity : BaseEntity
{
    private LoanPaymentEntity() { }
    private LoanPaymentEntity(Guid loanId, decimal amount)
    {
        LoanId = loanId;
        Amount = amount;
        PaidAt = DateTime.Now;
    }
    public Guid LoanId { get; private set; }
    public decimal Amount { get; private set; } 
    public DateTime PaidAt { get; private set; }

    public LoanEntity LoanEntity { get; set; } = null!;
    
    public static Result<LoanPaymentEntity> Create(Guid loanId, decimal amount)
    {
        if (loanId == Guid.Empty)
            return Result.Failure<LoanPaymentEntity>("Id is required");

        if (amount <= 0)
            return Result.Failure<LoanPaymentEntity>("Sum of payment must be > 0");

        var payment = new LoanPaymentEntity(loanId, amount);

        return Result.Success(payment);
    }
}