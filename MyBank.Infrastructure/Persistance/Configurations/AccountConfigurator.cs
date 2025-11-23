using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistance.Configurations;

public class AccountConfigurator : IEntityTypeConfiguration<AccountEntity> 
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance).HasPrecision(18,2);

        builder.Property(x => x.AccountType).HasConversion<string>();
        builder.Property(x => x.Status).HasConversion<string>();

        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.AccountNumber).HasMaxLength(30).IsRequired();

        builder.HasIndex(x => x.AccountNumber).IsUnique();

        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}