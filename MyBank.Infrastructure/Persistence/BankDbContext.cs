using System.Reflection;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistance.Configurations;

namespace MyBank.Infrastructure.Persistance;


public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
    {
    }
    
    public DbSet<UserEntity?> Users { get; set; } = null!;
    public DbSet<AccountEntity> Accounts { get; set; } = null!;
    public DbSet<LoanEntity> Loans { get; set; } = null!;
    public DbSet<LoanPaymentEntity> LoanPayments { get; set; } = null!;
    public DbSet<TransactionEntity> Transactions { get; set; } = null!;
    public DbSet<CardEntity> Cards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);

        modelBuilder.Entity<UserEntity>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<AccountEntity>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<CardEntity>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<LoanEntity>().HasQueryFilter(x => !x.IsDeleted);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && 
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.Now;
            }

            entity.UpdatedAt = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}