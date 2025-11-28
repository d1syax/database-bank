using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Configurations;

public class TransactionConfigurator : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Description).HasMaxLength(256);

        builder.Property(x => x.TransactionType).HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();
        
        builder.Ignore(x => x.IsCompleted);
        builder.Ignore(x => x.IsInternal);
        
        builder.HasIndex(x => x.CreatedAt);
        
        builder.HasIndex(x => new { x.CreatedAt, x.TransactionType });
        builder.HasIndex(x => new { x.FromAccountId, x.Status });
        
        builder.HasIndex(x => x.FromAccountId); 
        builder.HasIndex(x => x.ToAccountId);   
        
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Transaction_Amount_Positive", 
            "\"Amount\" > 0"));

        builder.HasOne(x => x.FromAccount)
            .WithMany(x => x.OutgoingTransactions)
            .HasForeignKey(x => x.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToAccount)
            .WithMany(x => x.IncomingTransactions)
            .HasForeignKey(x => x.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}