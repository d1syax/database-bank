using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Configurations;

public class LoanPaymentConfigurator : IEntityTypeConfiguration<LoanPaymentEntity>
{
    public void Configure(EntityTypeBuilder<LoanPaymentEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.HasOne(x => x.LoanEntity)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.LoanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}