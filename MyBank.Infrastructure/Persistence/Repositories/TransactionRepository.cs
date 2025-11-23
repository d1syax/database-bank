using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistence;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    public TransactionRepository(BankDbContext context)
    {
        _context = context;
    }

    private readonly BankDbContext _context;
    
    public async Task<TransactionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<TransactionEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.Where(x => x.FromAccountId == accountId || x.ToAccountId == accountId)
            .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
;   }

    public async Task<List<TransactionEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.Where(x => x.FromAccount.UserId == userId || x.ToAccount.UserId == userId)
            .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<List<TransactionEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TransactionEntity transaction, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(transaction, cancellationToken);
    }

    public Task UpdateAsync(TransactionEntity transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Update(transaction);
        return Task.CompletedTask;
    }
}