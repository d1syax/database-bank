using FluentAssertions;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;
using Xunit;

namespace MyBank.Tests.Domain;

public class CardEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var accountId = Guid.NewGuid();
        var result = CardEntity.Create(accountId, CardType.Debit);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccountId.Should().Be(accountId);
        result.Value.Status.Should().Be(CardStatus.Active);
        result.Value.CardNumber.Should().HaveLength(16);
        result.Value.ExpirationDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void Block_ActiveCard_ShouldChangeStatusToBlocked()
    {
        var card = CardEntity.Create(Guid.NewGuid(), CardType.Debit).Value;

        var result = card.Block();

        result.IsSuccess.Should().BeTrue();
        card.Status.Should().Be(CardStatus.Blocked);
        card.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Block_AlreadyBlockedCard_ShouldFail()
    {
        var card = CardEntity.Create(Guid.NewGuid(), CardType.Debit).Value;
        card.Block();

        var result = card.Block();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already blocked");
    }

    [Fact]
    public void ChangeLimit_WithNegativeAmount_ShouldFail()
    {
        var card = CardEntity.Create(Guid.NewGuid(), CardType.Debit).Value;

        var result = card.ChangeLimit(-1000);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("negative");
    }

    [Fact]
    public void MaskedCardNumber_ShouldHideMostDigits()
    {
            var card = CardEntity.Create(Guid.NewGuid(), CardType.Debit).Value;
        var cardNumber = card.CardNumber;

        var masked = card.MaskedCardNumber;

        masked.Should().Contain("****");
        masked.Should().EndWith(cardNumber.Substring(cardNumber.Length - 4));
    }
}