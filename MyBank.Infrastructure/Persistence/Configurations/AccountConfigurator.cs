using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Configurations;

public class AccountConfigurator : IEntityTypeConfiguration<AccountEntity> 
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance).HasPrecision(18,2);

        builder.Property(x => x.AccountType).HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();

        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.AccountNumber).HasMaxLength(30).IsRequired();

        builder.HasIndex(x => x.AccountNumber).IsUnique();
        builder.HasIndex(x => x.UserId);
        
        builder.HasIndex(x => new { x.UserId, x.AccountType, x.Status });
        builder.HasIndex(x => new { x.Currency, x.Balance });

        builder.Ignore(x => x.IsActive);

        builder.Property(x => x.RowVersion).IsRowVersion();
        
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Account_Balance_NonNegative", 
            "[Balance] >= 0"));

        builder.HasOne(x => x.UserEntity)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}