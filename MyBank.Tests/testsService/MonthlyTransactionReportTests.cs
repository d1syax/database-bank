using FluentAssertions;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.DTOs.Responses;
using MyBank.Application.Services;
using MyBank.Domain.Constants;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;

namespace MyBank.Tests.testsService;

public class MonthlyTransactionReportTests : TestBase
{
    private readonly ReportService _reportService;
    private readonly UserService _userService;

    public MonthlyTransactionReportTests()
    {
        _reportService = new ReportService(Context);
        _userService = new UserService(UserRepository, AccountRepository, CardRepository, UnitOfWork);
    }

    [Fact]
    public async Task GetMonthlyReport_WithMultipleMonths_ShouldGroupCorrectly()
    {
        var user = await CreateTestUserAsync();
        var account1 = await CreateTestAccountAsync(user.Id, 10000m);
        var account2 = await CreateTestAccountAsync(user.Id, 10000m);
        
        var jan15 = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var jan20 = new DateTime(2024, 1, 20, 14, 0, 0, DateTimeKind.Utc);
        await CreateCompletedTransferAsync(account1.Id, account2.Id, 1000m, jan15);
        await CreateCompletedTransferAsync(account1.Id, account2.Id, 2000m, jan20);
        
        var feb10 = new DateTime(2024, 2, 10, 12, 0, 0, DateTimeKind.Utc);
        await CreateCompletedTransferAsync(account1.Id, account2.Id, 5000m, feb10);

        var result = await _reportService.GetMonthlyTransactionReportAsync(
            user.Id, 
            CancellationToken.None
        );
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        
        var janReport = result.Value.Last(); 
        janReport.Month.Month.Should().Be(1);
        janReport.Month.Year.Should().Be(2024);
        janReport.Currency.Should().Be(CurrencyConstants.UAH);
        janReport.TransactionCount.Should().Be(2);    
        janReport.TotalVolume.Should().Be(3000m);     
        janReport.AverageTransaction.Should().Be(1500m);
        
        var febReport = result.Value.First();
        febReport.Month.Month.Should().Be(2);
        febReport.TransactionCount.Should().Be(1);
        febReport.TotalVolume.Should().Be(5000m);
        febReport.AverageTransaction.Should().Be(5000m);
    }
    
    [Fact]
    public async Task GetMonthlyReport_OnlyUserTransactions_ShouldExcludeOtherUsers()
    {
        var user1 = await CreateTestUserAsync("user1@test.com");
        var user2 = await CreateTestUserAsync("user2@test.com");
        
        var account1 = await CreateTestAccountAsync(user1.Id, 10000m);
        var account2 = await CreateTestAccountAsync(user2.Id, 10000m);
        var account3 = await CreateTestAccountAsync(user2.Id, 10000m);
        
        var jan2024 = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        
        await CreateCompletedTransferAsync(account1.Id, account2.Id, 1000m, jan2024);
        
        await CreateCompletedTransferAsync(account2.Id, account3.Id, 5000m, jan2024);

        var result = await _reportService.GetMonthlyTransactionReportAsync(
            user1.Id, 
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        
        var report = result.Value.First();
        report.TotalVolume.Should().Be(1000m);
    }
    
    [Fact]
    public async Task GetMonthlyReport_NonExistentUser_ShouldReturnEmptyList()
    {
        var fakeUserId = Guid.NewGuid();

        var result = await _reportService.GetMonthlyTransactionReportAsync(

            fakeUserId, 
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
    
    private async Task<UserResponse> CreateTestUserAsync(string email = "test@example.com")
    {
        var request = new CreateUserRequest(
            "Test", "User", email, "+380991234567",
            "password123", new DateTime(1990, 1, 1)
        );
        var result = await _userService.RegisterAsync(request, CancellationToken.None);
        return result.Value;
    }

    private async Task<AccountEntity> CreateTestAccountAsync(
        Guid userId, 
        decimal initialBalance = 0,
        string currency = CurrencyConstants.UAH)
    {
        var account = AccountEntity.Create(userId, currency, AccountType.Debit).Value;
        if (initialBalance > 0)
        {
            account.Deposit(initialBalance);
        }
        await AccountRepository.AddAsync(account);
        await UnitOfWork.SaveChangesAsync();
        return account;
    }

    private async Task CreateCompletedTransferAsync(
        Guid fromAccountId, 
        Guid toAccountId, 
        decimal amount,
        DateTime? createdAt = null)
    {
        var transaction = TransactionEntity.Create(
            fromAccountId,
            toAccountId,
            amount,
            TransactionType.Transfer,
            $"Test transfer {amount}"
        ).Value;
        
        transaction.Complete();
        
        if (createdAt.HasValue)
        {
            var createdAtProperty = typeof(TransactionEntity)
                .BaseType! 
                .GetProperty("CreatedAt")!;
            createdAtProperty.SetValue(transaction, createdAt.Value);
        }
        
        await TransactionRepository.AddAsync(transaction);
        await UnitOfWork.SaveChangesAsync();
    }
}