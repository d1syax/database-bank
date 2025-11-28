using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Configurations;

public class UserConfigurator : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(50).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Ignore(x => x.FullName);
        builder.Ignore(x => x.Age);
        
        builder.ToTable(t => t.HasCheckConstraint("CK_User_Email_Format", "position('@' IN \"Email\") > 1"));

        builder.ToTable(t => t.HasCheckConstraint("CK_User_MinAge", "\"DateOfBirth\" <= (CURRENT_DATE - INTERVAL '14 years')"));
    }
}