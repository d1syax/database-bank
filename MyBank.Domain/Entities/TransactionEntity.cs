using CSharpFunctionalExtensions;
using MyBank.Domain.Common; 
using MyBank.Domain.Enums; 

namespace MyBank.Domain.Entities;

public class TransactionEntity : BaseEntity
{
    private TransactionEntity() { }
    private TransactionEntity(Guid? fromAccountId, Guid? toAccountId, decimal amount, TransactionType type, string description)
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

    public AccountEntity? FromAccount { get; set; }
    public AccountEntity? ToAccount { get; set; }

    public bool IsCompleted => Status == TransactionStatus.Completed;
    public bool IsInternal => FromAccountId.HasValue && ToAccountId.HasValue;
    
    public static Result<TransactionEntity> Create(Guid? fromAccountId, Guid? toAccountId, decimal amount, TransactionType type, string description = "")
    {
        if (amount <= 0)
        return Result.Failure<TransactionEntity>("TransactionEntity amount must be positive");
        if (fromAccountId == null && toAccountId == null)
            return Result.Failure<TransactionEntity>("Source or destination account must be provided");
        if (fromAccountId == toAccountId)
            return Result.Failure<TransactionEntity>("Source and destination accounts cannot be the same");
        
        var transaction = new TransactionEntity(fromAccountId, toAccountId, amount, type, description);
        return Result.Success(transaction);
    }

    public Result Complete()
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure("TransactionEntity is already completed");

        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Fail()
    {
        if (Status != TransactionStatus.Pending)
            return Result.Failure("TransactionEntity is already finalized"); 

        Status = TransactionStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        return Result.Success();
    }
}