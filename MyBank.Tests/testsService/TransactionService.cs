using FluentAssertions;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.DTOs.Responses;
using MyBank.Application.Services;
using MyBank.Domain.Constants;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;

namespace MyBank.Tests.Services;

public class TransactionServiceTests : TestBase
{
    private readonly TransactionService _transactionService;
    private readonly UserService _userService;

    public TransactionServiceTests()
    {
        _transactionService = new TransactionService(
            AccountRepository, 
            TransactionRepository, 
            UnitOfWork
        );
        
      _userService = new UserService(UserRepository, AccountRepository, CardRepository, UnitOfWork);
   }
    [Fact]
    public async Task TransferAsync_WithValidData_ShouldTransferMoneyAndCreateRecord()
    {
        var senderUser = await CreateTestUserAsync("sender@example.com");
        var receiverUser = await CreateTestUserAsync("receiver@example.com");

        var senderAccount = await CreateTestAccountAsync(senderUser.Id, CurrencyConstants.UAH, 1000m);
        var receiverAccount = await CreateTestAccountAsync(receiverUser.Id, CurrencyConstants.UAH, 0m);

        var request = new CreateTransferRequest(
            FromAccountId: senderAccount.Id,
            ToAccountId: receiverAccount.Id,
            Amount: 500m,
            Description: "Payment for services"
        );

        var result = await _transactionService.TransferAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        
        result.Value.Amount.Should().Be(500m);
        result.Value.FromAccountId.Should().Be(senderAccount.Id);
        result.Value.ToAccountId.Should().Be(receiverAccount.Id);
        result.Value.Status.Should().Be(TransactionStatus.Completed.ToString());

        var updatedSender = await AccountRepository.GetByIdAsync(senderAccount.Id);
        var updatedReceiver = await AccountRepository.GetByIdAsync(receiverAccount.Id);

        updatedSender!.Balance.Should().Be(500m);
        updatedReceiver!.Balance.Should().Be(500m);

        var transaction = await TransactionRepository.GetByIdAsync(result.Value.Id);
        transaction.Should().NotBeNull();
        transaction!.TransactionType.Should().Be(TransactionType.Transfer);
    }
    [Fact]
    public async Task TransferAsync_WithInvalidAccounts_ShouldFail()
    {
        var request = new CreateTransferRequest(
            FromAccountId: Guid.NewGuid(),
            ToAccountId: Guid.NewGuid(),
            Amount: 100m
        );

        var result = await _transactionService.TransferAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task TransferAsync_InsufficientFunds_ShouldFail()
    {
        var user = await CreateTestUserAsync();
        var senderAccount = await CreateTestAccountAsync(user.Id, CurrencyConstants.UAH, 100m);
        var receiverAccount = await CreateTestAccountAsync(user.Id, CurrencyConstants.UAH, 0m);

        var request = new CreateTransferRequest(
            FromAccountId: senderAccount.Id,
            ToAccountId: receiverAccount.Id,
            Amount: 500m,
            Description: "FDSFDAS"
        );

        var result = await _transactionService.TransferAsync(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Insufficient funds");

        var updatedSender = await AccountRepository.GetByIdAsync(senderAccount.Id);
        updatedSender!.Balance.Should().Be(100m);
    }

    private async Task<UserResponse> CreateTestUserAsync(string email = "test_trans@example.com")
    {
        var randomPhone = $"+3809{new Random().Next(10000000, 99999999)}";
        
        var request = new CreateUserRequest(
            "Danilo",
            "Boyko",
            email,
            randomPhone,
            "pass123",
            new DateTime(1995, 5, 20)
        );
        
        var result = await _userService.RegisterAsync(request, CancellationToken.None);
        return result.Value;
    }

    private async Task<AccountEntity> CreateTestAccountAsync(Guid userId, string currency, decimal initialBalance)
    {
        var account = AccountEntity.Create(userId, currency, AccountType.Debit).Value;
        
        if (initialBalance > 0)
        {
            account.Deposit(initialBalance);
        }
        
        await AccountRepository.AddAsync(account, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync(CancellationToken.None);
        
        return account;
    }
}


  
