using FluentAssertions;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.DTOs.Responses;
using MyBank.Application.Services;
using MyBank.Domain.Constants;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;

namespace MyBank.Tests.testsService;

public class LoanServiceTests : TestBase
{
    private readonly LoanService _loanService;
    private readonly UserService _userService;

    public LoanServiceTests()
    {
        _loanService = new LoanService(
            LoanRepository, 
            AccountRepository, 
            UserRepository, 
            TransactionRepository, 
            UnitOfWork
        );
        _userService = new UserService(UserRepository, AccountRepository, CardRepository, UnitOfWork);
    }
    
    [Fact]
    public async Task IssueLoanAsync_WithValidData_ShouldCreateLoanAndDepositMoney()
    {
        var user = await CreateTestUserAsync();
        var account = await CreateTestAccountAsync(user.Id);
    
        var initialBalance = account.Balance;
    
        var request = new CreateLoanRequest(
            UserId: user.Id,
            AccountId: account.Id,
            Amount: 10000m, 
            InterestRate: 15m   
        );
    
        var result = await _loanService.IssueLoanAsync(request, CancellationToken.None);
    
        result.IsSuccess.Should().BeTrue();
    
        result.Value.PrincipalAmount.Should().Be(10000m);
        result.Value.InterestAmount.Should().Be(1500m);  
        result.Value.TotalAmountToRepay.Should().Be(11500m);
        result.Value.PaidAmount.Should().Be(0);
        result.Value.Status.Should().Be(LoanStatus.Active.ToString());
    
        var updatedAccount = await AccountRepository.GetByIdAsync(account.Id);
        updatedAccount!.Balance.Should().Be(initialBalance + 10000m); 
    
        var transactions = await TransactionRepository.GetByAccountIdAsync(
            account.Id, 
            skip: 0, 
            take: 100, 
            CancellationToken.None);
        
        transactions.Should().ContainSingle(t => 
            t.TransactionType == TransactionType.LoanDisbursement && 
            t.Amount == 10000m
        );
    }
    
    [Fact]
    public async Task IssueLoanAsync_WithNonExistentUser_ShouldFail()
    {
        var fakeUserId = Guid.NewGuid();
        var fakeAccountId = Guid.NewGuid();
        
        var request = new CreateLoanRequest(fakeUserId, fakeAccountId, 5000m, 10m);

        var result = await _loanService.IssueLoanAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }


    [Fact]
    public async Task IssueLoanAsync_WithAccountNotBelongingToUser_ShouldFail()
    {
        var user1 = await CreateTestUserAsync();
        var user2 = await CreateTestUserAsync("boykoVOR@example.com");
        
        var accountUser1 = await CreateTestAccountAsync(user1.Id);

        var request = new CreateLoanRequest(
            user2.Id,         
            accountUser1.Id, 
            5000m, 
            10m
        );

        var result = await _loanService.IssueLoanAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("does not belong"); 
    }


    [Fact]
    public async Task RepayLoanAsync_WithValidAmount_ShouldUpdateLoanAndDeductFromAccount()
    {
        var user = await CreateTestUserAsync();
        var account = await CreateTestAccountAsync(user.Id, initialBalance: 15000m);
        
        var loanRequest = new CreateLoanRequest(user.Id, account.Id, 10000m, 10m);
        var loanResult = await _loanService.IssueLoanAsync(loanRequest, CancellationToken.None);
        var loanId = loanResult.Value.Id;
        
        var balanceAfterLoan = (await AccountRepository.GetByIdAsync(account.Id))!.Balance;
        balanceAfterLoan.Should().Be(25000m);

        var repayRequest = new RepayLoanRequest(loanId, 3000m);
        var result = await _loanService.RepayLoanAsync(repayRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        
        var updatedLoan = await LoanRepository.GetByIdAsync(loanId);
        updatedLoan!.PaidAmount.Should().Be(3000m);       
        updatedLoan.Status.Should().Be(LoanStatus.Active); 
        
        var updatedAccount = await AccountRepository.GetByIdAsync(account.Id);
        updatedAccount!.Balance.Should().Be(22000m);
    }
    private async Task<UserResponse> CreateTestUserAsync(string email = "test@example.com")
    {
        var request = new CreateUserRequest(
            "Test",
            "SerhioBoyko",
            email,
            "+380914884567",
            "passwordSecuRe",
            new DateTime(1990, 1, 1)
        );
        
        var result = await _userService.RegisterAsync(request, CancellationToken.None);
        return result.Value;
    }
    
    private async Task<AccountEntity> CreateTestAccountAsync(Guid userId, decimal initialBalance = 0)
    {
        var account = AccountEntity.Create(userId, CurrencyConstants.UAH, AccountType.Debit).Value;
        
        if (initialBalance > 0)
        {
            account.Deposit(initialBalance);
        }
        
        await AccountRepository.AddAsync(account, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync(CancellationToken.None);
        
        return account;
    }
}