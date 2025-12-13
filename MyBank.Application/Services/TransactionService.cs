using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;
using MyBank.Domain.Interfaces;
namespace MyBank.Application.Services;

public class TransactionService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(
        IAccountRepository accountRepository, 
        ITransactionRepository transactionRepository, 
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionResponse>> TransferAsync(CreateTransferRequest request, CancellationToken ct)
{
    var fromAccount = await _accountRepository.GetByIdAsync(request.FromAccountId, ct);
    var toAccount = await _accountRepository.GetByIdAsync(request.ToAccountId, ct);

    if (fromAccount == null || toAccount == null)
        return Result.Failure<TransactionResponse>("One of the accounts was not found");

    using var transaction = await _unitOfWork.BeginTransactionAsync();
    try
    {
        if (fromAccount.Id == toAccount.Id)
            return Result.Failure<TransactionResponse>("Cannot transfer to the same account");

        if (fromAccount.Currency != toAccount.Currency)
            return Result.Failure<TransactionResponse>("Currency mismatch");

        var withdrawResult = fromAccount.Withdraw(request.Amount);
        if (withdrawResult.IsFailure)
            return Result.Failure<TransactionResponse>(withdrawResult.Error);
        
        if (fromAccount.Currency != toAccount.Currency)
            return Result.Failure<TransactionResponse>($"Cannot transfer between different currencies");

        var depositResult = toAccount.Deposit(request.Amount);
        if (depositResult.IsFailure)
        {
            fromAccount.Deposit(request.Amount);
            return Result.Failure<TransactionResponse>(depositResult.Error);
        }

        var txResult = TransactionEntity.Create(
            fromAccount.Id, 
            toAccount.Id, 
            request.Amount, 
            TransactionType.Transfer, 
            request.Description
        );

        if (txResult.IsFailure)
            return Result.Failure<TransactionResponse>(txResult.Error);

        var tx = txResult.Value;
        tx.Complete();

        await _transactionRepository.AddAsync(tx, ct);
        await _accountRepository.UpdateAsync(fromAccount, ct);
        await _accountRepository.UpdateAsync(toAccount, ct);   
        
        await _unitOfWork.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Success(new TransactionResponse(
            tx.Id,
            tx.FromAccountId!.Value,
            tx.ToAccountId!.Value,
            tx.Amount,
            tx.TransactionType.ToString(),
            tx.Status.ToString(),
            tx.Description,
            tx.CreatedAt
        ));
    }
    catch (DbUpdateConcurrencyException)
    {
        await transaction.RollbackAsync(ct);
        return Result.Failure<TransactionResponse>(
            "Transfer failed. Please try again");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync(ct);
        return Result.Failure<TransactionResponse>($"{ex.Message}");
    }
}
    public async Task<List<TransactionResponse>> GetAccountHistoryAsync(Guid accountId, CancellationToken ct)
    {
        var transactions = await _transactionRepository.GetByAccountIdAsync(accountId, ct);
        
        var response = new List<TransactionResponse>();
        foreach (var t in transactions)
        {
            response.Add(new TransactionResponse(
                t.Id,
                t.FromAccountId ?? Guid.Empty,
                t.ToAccountId ?? Guid.Empty,
                t.Amount,
                t.TransactionType.ToString(),
                t.Status.ToString(),
                t.Description,
                t.CreatedAt
            ));
        }
        return response;
    }
}