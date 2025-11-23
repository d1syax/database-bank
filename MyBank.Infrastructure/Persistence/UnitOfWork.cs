using MyBank.Domain.Interfaces;

namespace MyBank.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BankDbContext _context;

    public UnitOfWork(BankDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }
}
