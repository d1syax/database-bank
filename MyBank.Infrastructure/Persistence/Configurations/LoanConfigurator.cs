using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistance.Configurations;

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