using System.Transactions;
using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class AccountEntity : SoftDeletableEntity
{
    private AccountEntity() { }
    private AccountEntity(Guid userId, string accountNumber, AccountType accountType, string currency)
    {
        UserId = userId;
        AccountNumber = accountNumber;
        AccountType = accountType;
        Currency = currency;
        Balance = 0;
        Status = AccountStatus.Active;
        OpenedAt = DateTime.UtcNow;
    }
    public Guid UserId { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public AccountType AccountType { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; } = "UAH";
    public AccountStatus Status { get; private set; }

    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    public byte[] RowVersion { get; set; } 

    public UserEntity UserEntity { get; set; } = null!;
    
    public ICollection<TransactionEntity> OutgoingTransactions { get; set; } = new List<TransactionEntity>();
    public ICollection<TransactionEntity> IncomingTransactions { get; set; } = new List<TransactionEntity>();
    public ICollection<CardEntity> Cards { get; set; } = new List<CardEntity>();
    public ICollection<LoanEntity> Loans { get; set; } = new List<LoanEntity>();

    public bool IsActive => Status == AccountStatus.Active;
 
    public static Result<AccountEntity> Create(Guid userId, string currency, AccountType accountType)
    {
        if (userId == Guid.Empty) 
            return Result.Failure<AccountEntity>("UserId is required");
            
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3) 
            return Result.Failure<AccountEntity>("Invalid currency code");

        var rnd = new Random();
        var accountNumber = $"UA{DateTime.UtcNow:yyMM}{Guid.NewGuid().ToString("N").Substring(0, 16)}";

        var account = new AccountEntity(userId, accountNumber, accountType, currency);

        return Result.Success(account);
    }
    
    public Result Deposit(decimal amount)
    {
        if (amount <= 0) 
            return Result.Failure("Deposit amount must be positive");

        if (!IsActive)
            return Result.Failure("AccountEntity is not active");

        Balance += amount;
        return Result.Success();
    }

    public Result Withdraw(decimal amount)
    {
        if (amount <= 0) 
            return Result.Failure("Withdraw amount must be positive");
            
        if (!IsActive)
            return Result.Failure("AccountEntity is not active");

        if (AccountType != AccountType.Credit && Balance < amount)
            return Result.Failure("Insufficient funds");

        Balance -= amount;
        return Result.Success();
    }

    public Result Close()
    {
        Status = AccountStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        return Result.Success();
    }
}