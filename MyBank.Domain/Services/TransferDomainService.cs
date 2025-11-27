using CSharpFunctionalExtensions;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;

namespace MyBank.Domain.Services;

public class TransferDomainService
{
    public Result<TransactionEntity> Transfer(AccountEntity fromAccount, AccountEntity toAccount, decimal amount, string description)
    {
        if (fromAccount.Id == toAccount.Id)
            return Result.Failure<TransactionEntity>("Cannot transfer money to the same account");

        if (fromAccount.Currency != toAccount.Currency)
            return Result.Failure<TransactionEntity>("Transfers between different currencies are not supported yet");

        var withdrawResult = fromAccount.Withdraw(amount);
        if (withdrawResult.IsFailure)
            return Result.Failure<TransactionEntity>($"{withdrawResult.Error}");

        var depositResult = toAccount.Deposit(amount);
        if (depositResult.IsFailure)
        {
            return Result.Failure<TransactionEntity>($"{depositResult.Error}");
        }

        var transactionResult = TransactionEntity.Create(
            fromAccount.Id, 
            toAccount.Id, 
            amount, 
            TransactionType.Transfer, 
            description
        );

        if (transactionResult.IsFailure)
            return Result.Failure<TransactionEntity>(transactionResult.Error);

        var transaction = transactionResult.Value;
        transaction.Complete(); 
        return Result.Success(transaction);
    }
}