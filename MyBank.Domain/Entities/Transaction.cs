using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class Transaction : BaseEntity
{
    private Transaction() { }
    private Transaction(Guid? fromAccountId, Guid? toAccountId, decimal amount, TransactionType type, string description)
    {
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
        Amount = amount;
        TransactionType = type;
        Description = description;
        Status = TransactionStatus.Pending;
    }
    public Guid? FromAccountId { get; private set; } 
    public Guid? ToAccountId { get; private set; } 
        
    public decimal Amount { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime? CompletedAt { get; private set; }

    public Account? FromAccount { get; set; }
    public Account? ToAccount { get; set; }

    public bool IsCompleted => Status == TransactionStatus.Completed;
    public bool IsInternal => FromAccountId.HasValue && ToAccountId.HasValue;
    
    public static Result<Transaction> Create(Guid? fromAccountId, Guid? toAccountId, decimal amount, TransactionType type, string description = "")
    {
        if (amount <= 0)
        return Result.Failure<Transaction>("Transaction amount must be positive");

        if (fromAccountId == null && toAccountId == null)
            return Result.Failure<Transaction>("Source or destination account must be provided");

        if (fromAccountId == toAccountId)
            return Result.Failure<Transaction>("Source and destination accounts cannot be the same");

        var transaction = new Transaction(fromAccountId, toAccountId, amount, type, description);

        return Result.Success(transaction);
    }

    public void Complete()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Transaction is already completed or cancelled");

        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.Now;
    }

    public void Fail()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Transaction is already finalized"); 

        Status = TransactionStatus.Failed;
        CompletedAt = DateTime.Now;
    }
}