using System.Transactions;
using CSharpFunctionalExtensions;
using DefaultNamespace;

namespace MyBank.Domain.Entities;

public class Account : SoftDeletableEntity
{
    private Account() { }
    private Account(Guid userId, string accountNumber, AccountType accountType, string currency)
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

    public User User { get; set; } = null!;
    
    public ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
    public ICollection<Card> Cards { get; set; } = new List<Card>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public bool IsActive => Status == AccountStatus.Active;
 
    public static Result<Account> Create(Guid userId, string currency, AccountType accountType)
    {
        if (userId == Guid.Empty) 
            return Result.Failure<Account>("UserId is required");
            
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3) 
            return Result.Failure<Account>("Invalid currency code");

        var rnd = new Random();
        var accountNumber = $"UA{rnd.Next(10, 99)}735692{rnd.NextInt64(1000000000, 9999999999)}";

        var account = new Account(userId, accountNumber, accountType, currency);

        return Result.Success(account);
    }
    
    public Result Deposit(decimal amount)
    {
        if (amount <= 0) 
            return Result.Failure("Deposit amount must be positive");

        if (!IsActive)
            return Result.Failure("Account is not active");

        Balance += amount;
        return Result.Success();
    }

    public Result Withdraw(decimal amount)
    {
        if (amount <= 0) 
            return Result.Failure("Withdraw amount must be positive");
            
        if (!IsActive)
            return Result.Failure("Account is not active");

        if (AccountType != AccountType.Credit && Balance < amount)
            return Result.Failure("Insufficient funds");

        Balance -= amount;
        return Result.Success();
    }

    public void Close()
    {
        Status = AccountStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }
}