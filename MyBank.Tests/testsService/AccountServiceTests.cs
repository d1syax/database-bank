using FluentAssertions;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Application.Services;
using MyBank.Domain.Constants;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;
using Xunit;

namespace MyBank.Tests.Services;

public class AccountServiceTests : TestBase
{
    private readonly AccountService _accountService;
    private readonly UserService _userService;

    public AccountServiceTests()
    {
        _accountService = new AccountService(AccountRepository, UserRepository, UnitOfWork, TransactionRepository);
        _userService = new UserService(UserRepository, AccountRepository, CardRepository, UnitOfWork);
    }

    [Fact]
    public async Task CreateAccountAsync_WithValidData_ShouldSucceed()
    {
        var user = await CreateTestUserAsync(); 
        var request = new CreateAccountRequest(user.Id, CurrencyConstants.USD, AccountType.Savings);
        
        var result = await _accountService.CreateAccountAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Currency.Should().Be(CurrencyConstants.USD);
        result.Value.AccountType.Should().Be(AccountType.Savings.ToString());
    }

    [Fact]
    public async Task CreateAccountAsync_WithNonExistentUser_ShouldFail()
    {
        var request = new CreateAccountRequest(Guid.NewGuid(), CurrencyConstants.UAH, AccountType.Debit);

        var result = await _accountService.CreateAccountAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task OpenDepositAccountAsync_WithSufficientFunds_ShouldWork()
    {
        var user = await CreateTestUserAsync();
        
        var mainAccount = AccountEntity.Create(user.Id, CurrencyConstants.UAH, AccountType.Debit).Value;
        mainAccount.Deposit(10000); 
        
        await AccountRepository.AddAsync(mainAccount, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync();

        var request = new CreateDepositRequest(user.Id, mainAccount.Id, 5000, CurrencyConstants.UAH);

        var result = await _accountService.OpenDepositAccountAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var updatedMain = await AccountRepository.GetByIdAsync(mainAccount.Id);
        updatedMain!.Balance.Should().Be(5000); 

        var depositAccount = await AccountRepository.GetByIdAsync(result.Value);
        depositAccount!.Balance.Should().Be(5000);
        depositAccount.AccountType.Should().Be(AccountType.Savings);
    }

    [Fact]
    public async Task OpenDepositAccountAsync_WithInsufficientFunds_ShouldFail()
    {
        var user = await CreateTestUserAsync();
        
        var mainAccount = AccountEntity.Create(user.Id, CurrencyConstants.UAH, AccountType.Debit).Value;
        await AccountRepository.AddAsync(mainAccount, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync();

        var request = new CreateDepositRequest(user.Id, mainAccount.Id, 5000, CurrencyConstants.UAH);

        var result = await _accountService.OpenDepositAccountAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("enough");
    }

    [Fact]
    public async Task CloseAccountAsync_ShouldChangeStatusToClosed()
    {
        var user = await CreateTestUserAsync();
        var account = AccountEntity.Create(user.Id, CurrencyConstants.UAH, AccountType.Debit).Value;
        await AccountRepository.AddAsync(account, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync();

        var result = await _accountService.CloseAccountAsync(account.Id, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        var closedAccount = await AccountRepository.GetByIdAsync(account.Id);
        closedAccount!.Status.Should().Be(AccountStatus.Closed);
    }
    
    private async Task<UserResponse> CreateTestUserAsync()
    {
        var request = new CreateUserRequest(
            "Danilo",
            "Boyko",
            $"boyaka201@gmail.com",
            "+380979734812",
            "pass123",
            new DateTime(2006, 12, 28)
        );
        var result = await _userService.RegisterAsync(request, CancellationToken.None);
        return result.Value;
    }
}