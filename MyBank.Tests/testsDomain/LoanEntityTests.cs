using FluentAssertions;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;
using Xunit;

namespace MyBank.Tests.Domain;

public class LoanEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var amount = 10000m; 
        var interestRate = 15m;
        
        var result = LoanEntity.Create(userId, accountId, amount, interestRate);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.AccountId.Should().Be(accountId);
        result.Value.PrincipalAmount.Should().Be(amount);
        result.Value.InterestAmount.Should().Be(1500m);
        result.Value.TotalAmountToRepay.Should().Be(11500m);
        result.Value.PaidAmount.Should().Be(0); 
        result.Value.Status.Should().Be(LoanStatus.Active);
        result.Value.IsFullyPaid.Should().BeFalse();
    }
    
    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        var emptyUserId = Guid.Empty;
        var accountId = Guid.NewGuid();
        
        var result = LoanEntity.Create(emptyUserId, accountId, 1000m, 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("UserId");
    }
    
    [Fact]
    public void Create_WithEmptyAccountId_ShouldFail()
    {
        var userId = Guid.NewGuid();
        var emptyAccountId = Guid.Empty;
        
        var result = LoanEntity.Create(userId, emptyAccountId, 1000m, 10m);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("AccountId");
    }
    
    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var negativeAmount = -1000m;

        var result = LoanEntity.Create(userId, accountId, negativeAmount, 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("positive");
    }
    
    [Fact]
    public void Create_WithZeroAmount_ShouldFail()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var result = LoanEntity.Create(userId, accountId, 0m, 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("positive");
    }
    
    [Fact]
    public void Create_WithNegativeInterestRate_ShouldFail()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        
        var result = LoanEntity.Create(userId, accountId, 1000m, -5m);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("negative");
    }
    
    [Fact]
    public void Create_WithZeroInterestRate_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var amount = 5000m;

        var result = LoanEntity.Create(userId, accountId, amount, 0m);

        result.IsSuccess.Should().BeTrue();
        result.Value.InterestAmount.Should().Be(0);
        result.Value.TotalAmountToRepay.Should().Be(amount);
    }
}