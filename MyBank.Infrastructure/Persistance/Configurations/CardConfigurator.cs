using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistance.Configurations;

public class CardConfigurator : IEntityTypeConfiguration<CardEntity>
{
    public void Configure(EntityTypeBuilder<CardEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CardNumber).HasMaxLength(16).IsRequired();
        builder.Property(x => x.CVV).HasMaxLength(3).IsRequired();
        builder.Property(x => x.DailyLimit).HasPrecision(18, 2);
        
        builder.Property(x => x.CardType).HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();

        builder.HasIndex(x => x.CardNumber).IsUnique();

        builder.HasOne(x => x.AccountEntity)
            .WithMany(x => x.Cards)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}