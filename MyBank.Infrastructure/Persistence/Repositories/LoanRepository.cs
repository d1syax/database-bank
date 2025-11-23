using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistence;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class LoanRepository : ILoanRepository
{
    public LoanRepository(BankDbContext context)
    {
        _context = context;
    }

    private readonly BankDbContext _context;
    
    public async Task<LoanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Loans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<LoanEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<List<LoanEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans.Where(x => x.AccountId == accountId).ToListAsync(cancellationToken);
    }

    public async Task<LoanEntity?> GetByIdWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Loans.Include(x => x.Payments).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<LoanEntity>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var overDue = DateTime.Now.AddDays(-30);

        return await _context.Loans.Where(x => x.Status == LoanStatus.Active && x.IssuedAt < overDue).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LoanEntity loan, CancellationToken cancellationToken = default)
    {
        await _context.Loans.AddAsync(loan);
    }

    public Task UpdateAsync(LoanEntity loan, CancellationToken cancellationToken = default)
    {
        _context.Loans.Update(loan);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(LoanEntity loan, CancellationToken cancellationToken = default)
    {
        loan.IsDeleted = true;
        _context.Loans.Update(loan);
        return Task.CompletedTask;
    }
}