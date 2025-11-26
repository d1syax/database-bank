using CSharpFunctionalExtensions;
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
}