using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Configurations;

public class LoanConfigurator : IEntityTypeConfiguration<LoanEntity>
{
    public void Configure(EntityTypeBuilder<LoanEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PrincipalAmount).HasPrecision(18, 2);
        builder.Property(x => x.InterestAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();

        builder.Ignore(x => x.TotalAmountToRepay);
        builder.Ignore(x => x.IsFullyPaid);
        
        builder.HasIndex(x => new { x.UserId, x.Status });
        
        builder.HasIndex(x => x.AccountId);
        
        builder.ToTable(t => 
        {
            t.HasCheckConstraint("CK_Loan_PrincipalAmount_Positive", "[PrincipalAmount] > 0");
            t.HasCheckConstraint("CK_Loan_InterestAmount_NonNegative", "[InterestAmount] >= 0");
            t.HasCheckConstraint("CK_Loan_PaidAmount_NonNegative", "[PaidAmount] >= 0");
        });

        builder.HasOne(x => x.UserEntity)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AccountEntity)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}