using System.Transactions;
using CSharpFunctionalExtensions;
using DefaultNamespace;
using MyBank.Domain.Entities;

namespace MyBank.Domain.Services;

public class LoanDomainService
{
    public Result<(LoanEntity Loan, TransactionEntity Transaction)> IssueLoan(UserEntity user, AccountEntity targetAccount, decimal amount, decimal interestRate)
    {
        if (targetAccount.UserId != user.Id)
            return Result.Failure<(LoanEntity, TransactionEntity)>("The target account does not belong to the user.");
            
        if (!targetAccount.IsActive)
            return Result.Failure<(LoanEntity, TransactionEntity)>("Account is blocked or inactive, cannot issue a loan.");

        var loanResult = LoanEntity.Create(user.Id, targetAccount.Id, amount, interestRate);
        if (loanResult.IsFailure)
            return Result.Failure<(LoanEntity, TransactionEntity)>(loanResult.Error);

        var loan = loanResult.Value;

        var depositResult = targetAccount.Deposit(amount);
        if (depositResult.IsFailure)
        {
            return Result.Failure<(LoanEntity, TransactionEntity)>($"Failed to deposit loan funds: {depositResult.Error}");
        }

        var transactionResult = TransactionEntity.Create(
            null,
            targetAccount.Id, 
            amount,
            TransactionType.LoanDisbursement,
            $"Rate: {interestRate}%"
        );
        
        if (transactionResult.IsFailure)
            return Result.Failure<(LoanEntity, TransactionEntity)>(transactionResult.Error);

        var transaction = transactionResult.Value;
        transaction.Complete();

        return Result.Success((loan, transaction));
    }
}