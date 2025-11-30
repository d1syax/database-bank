using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Domain.Enums;

namespace MyBank.Application.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionRepository _transactionRepository;

    public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _transactionRepository = transactionRepository;
    }
    
    public async Task<List<AccountResponse>> GetUserAccountsAsync(Guid userId, CancellationToken ct)
    {
        var accounts = await _accountRepository.GetByUserIdAsync(userId, ct);

        var response = new List<AccountResponse>();
        foreach (var acc in accounts)
        {
            response.Add(new AccountResponse(
                acc.Id, acc.UserId, acc.AccountNumber, acc.Balance,
                acc.Currency, acc.AccountType.ToString(), acc.Status.ToString(), acc.OpenedAt
            ));
        }
        return response;
    }

    public async Task<Result<AccountResponse>> CreateAccountAsync(CreateAccountRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null) 
            return Result.Failure<AccountResponse>("User not found");

        var accountResult = AccountEntity.Create(request.UserId, request.Currency, request.AccountType);
        if (accountResult.IsFailure) 
            return Result.Failure<AccountResponse>(accountResult.Error);

        var account = accountResult.Value;
        await _accountRepository.AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AccountResponse(
            account.Id, account.UserId, account.AccountNumber, account.Balance,
            account.Currency, account.AccountType.ToString(), account.Status.ToString(), account.OpenedAt
        ));
    }

    public async Task<Result> CloseAccountAsync(Guid accountId, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, ct);
        if (account == null) 
            return Result.Failure("Account not found");

        var result = account.Close();
        if (result.IsFailure) 
            return result;

        await _accountRepository.UpdateAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
    
    public async Task<Result<AccountResponse>> UpdateBalanceWithConcurrencyCheckAsync(
        Guid accountId, 
        decimal amountToAdd, 
        CancellationToken ct)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var account = await _accountRepository.GetByIdAsync(accountId, ct);
            if (account == null)
                return Result.Failure<AccountResponse>("Account not found");

            var originalRowVersion = account.RowVersion;
            
            if (amountToAdd > 0)
            {
                var depositResult = account.Deposit(amountToAdd);
                if (depositResult.IsFailure)
                    return Result.Failure<AccountResponse>(depositResult.Error);
            }
            else
            {
                var withdrawResult = account.Withdraw(Math.Abs(amountToAdd));
                if (withdrawResult.IsFailure)
                    return Result.Failure<AccountResponse>(withdrawResult.Error);
            }
            await _unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success(new AccountResponse(
                account.Id, account.UserId, account.AccountNumber, account.Balance,
                account.Currency, account.AccountType.ToString(), 
                account.Status.ToString(), account.OpenedAt
            ));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            return Result.Failure<AccountResponse>(
                "Update failed. Please try again");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            return Result.Failure<AccountResponse>($"{ex.Message}");
        }
    }
    
    public async Task<List<AccountResponse>> GetAccountsAboveBalanceAsync(Guid userId, decimal minBalance, CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetByUserIdAsync(userId, cancellationToken);
    
        var filtered = accounts
            .Where(x => x.Balance >= minBalance && x.Status == Domain.Enums.AccountStatus.Active)
            .OrderByDescending(x => x.Balance)
            .Select(x => new AccountResponse(
                x.Id, x.UserId, x.AccountNumber, x.Balance,
                x.Currency, x.AccountType.ToString(), x.Status.ToString(), x.OpenedAt))
            .ToList();

        return filtered;
    }

    public async Task<Result<Guid>> OpenDepositAccountAsync(CreateDepositRequest request, CancellationToken ct)
    {
        if (request.Amount <= 0)
        {
            return Result.Failure<Guid>("Deposit amount can't be less then 0");
        }

        var fromAccount = await _accountRepository.GetByIdAsync(request.AccountId, ct);
        if (fromAccount == null)
        {
            return Result.Failure<Guid>("Account not found");
        }

        if (fromAccount.Balance < request.Amount)
        {
            return Result.Failure<Guid>("Not enough money to open deposit");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var newAccountResult = AccountEntity.Create(
                request.UserId, 
                request.Currency, 
                AccountType.Savings
                );
            
            if (newAccountResult.IsFailure)
            {
                return Result.Failure<Guid>(newAccountResult.Error);
            }
            
            var newAccount = newAccountResult.Value;

            var withdrawResult = fromAccount.Withdraw(request.Amount);
            if (withdrawResult.IsFailure)
            {
                return Result.Failure<Guid>(withdrawResult.Error);
            }
            
            var depositResult = newAccount.Deposit(request.Amount);
            if (depositResult.IsFailure)
            {
                return Result.Failure<Guid>(depositResult.Error);
            }

            var txResult = TransactionEntity.Create(
                fromAccount.Id,
                newAccount.Id,
                request.Amount,
                TransactionType.Transfer,
                "Deposit transfer"
            );

            if (txResult.IsFailure)
            {
                return Result.Failure<Guid>(txResult.Error);
            }
            var tx = txResult.Value;
            tx.Complete();

            await _accountRepository.AddAsync(newAccount, ct);
            await _accountRepository.UpdateAsync(fromAccount, ct);
            await _transactionRepository.AddAsync(tx, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success(newAccount.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>($"Transaction failed: {ex.Message}");
        }
    }
}