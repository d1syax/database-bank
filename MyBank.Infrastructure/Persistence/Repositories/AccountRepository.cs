using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistence;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    public AccountRepository(BankDbContext context)
    {
        _context = context;
    }

    private readonly BankDbContext _context;
    
    public async Task<AccountEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<AccountEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<AccountEntity?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task AddAsync(AccountEntity account, CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
    }

    public Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AccountEntity account, CancellationToken cancellationToken = default)
    {
        account.IsDeleted = true;
        _context.Accounts.Update(account);
        return Task.CompletedTask;
    }
} 