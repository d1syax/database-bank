using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;

namespace MyBank.Application.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
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
}