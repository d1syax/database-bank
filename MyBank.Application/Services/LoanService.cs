
using CSharpFunctionalExtensions;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Domain.Services;

namespace MyBank.Application.Services;

public class LoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LoanDomainService _loanDomainService;

    public LoanService(ILoanRepository loanRepository, IAccountRepository accountRepository, IUserRepository userRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _loanDomainService = new LoanDomainService();
    }
    
    public async Task<List<LoanResponse>> GetUserLoansAsync(Guid userId, CancellationToken ct)
    {
        var loans = await _loanRepository.GetByUserIdAsync(userId, ct);
        
        var response = new List<LoanResponse>();
        foreach (var loan in loans)
        {
            response.Add(new LoanResponse(
                loan.Id, 
                loan.PrincipalAmount, 
                loan.InterestAmount, 
                loan.PaidAmount,
                loan.TotalAmountToRepay, 
                loan.Status.ToString(), 
                loan.IssuedAt
            ));
        }
        return response;
    }

    public async Task<Result<LoanResponse>> IssueLoanAsync(CreateLoanRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct);

        if (user == null) 
            return Result.Failure<LoanResponse>("User not found");
        if (account == null) 
            return Result.Failure<LoanResponse>("Account not found");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var result = _loanDomainService.IssueLoan(user, account, request.Amount, request.InterestRate);
            if (result.IsFailure) 
                return Result.Failure<LoanResponse>(result.Error);

            var (newLoan, disbursementTx) = result.Value;

            await _loanRepository.AddAsync(newLoan, ct);
            await _transactionRepository.AddAsync(disbursementTx, ct);
            
            await _unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success(new LoanResponse(
                newLoan.Id, newLoan.PrincipalAmount, newLoan.InterestAmount, newLoan.PaidAmount,
                newLoan.TotalAmountToRepay, newLoan.Status.ToString(), newLoan.IssuedAt
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<LoanResponse>($"{ex.Message}");
        }
    }

    public async Task<Result> RepayLoanAsync(RepayLoanRequest request, CancellationToken ct)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId, ct);
        if (loan == null) 
            return Result.Failure("Loan not found");

        var account = await _accountRepository.GetByIdAsync(loan.AccountId, ct);
        if (account == null) 
            return Result.Failure("Linked account not found");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var withdrawResult = account.Withdraw(request.Amount);
            if (withdrawResult.IsFailure) 
                return Result.Failure(withdrawResult.Error);

            var repayResult = loan.Repay(request.Amount);
            if (repayResult.IsFailure) 
                return Result.Failure(repayResult.Error);

            await _loanRepository.UpdateAsync(loan, ct);
            await _accountRepository.UpdateAsync(account, ct);
            
            await _unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"{ex.Message}");
        }
    }
    
    public async Task<Result> ArchiveLoanAsync(Guid loanId, CancellationToken ct)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId, ct);
        if (loan == null)
        {
            return Result.Failure("Loan not found");

        }
        
        if (loan.Status != Domain.Enums.LoanStatus.Paid)
        {
            return Result.Failure("Cannot delete an active loan.");
        }

        await _loanRepository.DeleteAsync(loan, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}