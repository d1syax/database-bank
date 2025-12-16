using FluentAssertions;
using MyBank.Domain.Entities;
using MyBank.Domain.Enums;

namespace MyBank.Tests.Domain;

public class TransactionEntityTests
{
        [Fact]
        public void Create_WithBothAccounts_ShouldSucceed()
        {
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var amount = 1000m;
            var description = "Transfer to Serhiy Boyko";
            
            var result = TransactionEntity.Create(
                fromAccountId,       
                toAccountId, 
                amount,
                TransactionType.Transfer,
                description
            );

            result.IsSuccess.Should().BeTrue();
            result.Value.FromAccountId.Should().Be(fromAccountId);
            result.Value.ToAccountId.Should().Be(toAccountId);
            result.Value.Amount.Should().Be(amount);
            result.Value.TransactionType.Should().Be(TransactionType.Transfer);
            result.Value.Status.Should().Be(TransactionStatus.Pending);
            result.Value.Description.Should().Be(description);
            result.Value.IsCompleted.Should().BeFalse();
            result.Value.IsInternal.Should().BeTrue();
        }
        
        [Fact]
        public void Create_WithOnlyToAccount_ShouldSucceed()
        {
            var toAccountId = Guid.NewGuid();

            var result = TransactionEntity.Create(
                null,                     
                toAccountId,                    
                5000m,
                TransactionType.LoanDisbursement, 
                "Take Loan"
            );

            result.IsSuccess.Should().BeTrue();
            result.Value.FromAccountId.Should().BeNull();     
            result.Value.ToAccountId.Should().Be(toAccountId);
            result.Value.IsInternal.Should().BeFalse(); 
        }
        
        [Fact]
        public void Create_WithNegativeAmount_ShouldFail()
        {
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var result = TransactionEntity.Create(
                fromAccountId,
                toAccountId,
                -500m,                  
                TransactionType.Transfer,
                "Negative transfer"
            );
            
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("positive");
        }
        
        [Fact]
        public void Create_WithZeroAmount_ShouldFail()
        {
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            var result = TransactionEntity.Create(
                fromAccountId,
                toAccountId,
                0m,                         
                TransactionType.Transfer,
                "Zero transfer????"
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("positive");
        }
        
        [Fact]
        public void IsCompleted_ForFailedTransaction_ShouldBeFalse()
        {
            var transaction = TransactionEntity.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000m,
                TransactionType.Transfer,
                "Test"
            ).Value;
            
            transaction.Fail();

            transaction.Status.Should().Be(TransactionStatus.Failed);
            transaction.IsCompleted.Should().BeFalse();
        }
}