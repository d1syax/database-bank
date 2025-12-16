using FluentAssertions;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;
using MyBank.Domain.Constants;
using Xunit;

namespace MyBank.Tests.Domain;

public class AccountEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var userId = Guid.NewGuid();
        var result = AccountEntity.Create(userId, CurrencyConstants.UAH, AccountType.Debit);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.Currency.Should().Be(CurrencyConstants.UAH);
        result.Value.Balance.Should().Be(0);
        result.Value.Status.Should().Be(AccountStatus.Active);
    }

    [Fact]
    public void Create_WithUnsupportedCurrency_ShouldFail()
    {
        var result = AccountEntity.Create(Guid.NewGuid(),"GBP", AccountType.Debit);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Unsupported");
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        var account = AccountEntity.Create(Guid.NewGuid(), CurrencyConstants.UAH, AccountType.Debit).Value;

        var result = account.Deposit(1000);

        result.IsSuccess.Should().BeTrue();
        account.Balance.Should().Be(1000);
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ShouldFail()
    {
        var account = AccountEntity.Create(Guid.NewGuid(), CurrencyConstants.UAH, AccountType.Debit).Value;
        account.Deposit(100);

        var result = account.Withdraw(500);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Insufficient");
    }

    [Fact]
    public void Close_ShouldChangeStatusToClosed()
    {
        var account = AccountEntity.Create(Guid.NewGuid(), CurrencyConstants.UAH, AccountType.Debit).Value;

        var result = account.Close();

        result.IsSuccess.Should().BeTrue();
        account.Status.Should().Be(AccountStatus.Closed);
        account.ClosedAt.Should().NotBeNull();
        account.IsActive.Should().BeFalse();
    }
}