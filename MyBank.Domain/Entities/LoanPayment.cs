using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class LoanPayment : BaseEntity
{
    private LoanPayment() { }
    private LoanPayment(Guid loanId, decimal amount)
    {
        LoanId = loanId;
        Amount = amount;
        PaidAt = DateTime.Now;
    }
    public Guid LoanId { get; private set; }
    public decimal Amount { get; private set; } 
    public DateTime PaidAt { get; private set; }

    public Loan Loan { get; set; } = null!;
    
    public static Result<LoanPayment> Create(Guid loanId, decimal amount)
    {
        if (loanId == Guid.Empty)
            return Result.Failure<LoanPayment>("Id is required");

        if (amount <= 0)
            return Result.Failure<LoanPayment>("Sum of payment must be > 0");

        var payment = new LoanPayment(loanId, amount);

        return Result.Success(payment);
    }
}