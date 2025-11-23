using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ILoanRepository
{
    Task<LoanEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<LoanEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<LoanEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<LoanEntity?> GetByIdWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<LoanEntity>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task AddAsync(LoanEntity loan, CancellationToken cancellationToken = default);
    Task UpdateAsync(LoanEntity loan, CancellationToken cancellationToken = default);
    Task DeleteAsync(LoanEntity loan, CancellationToken cancellationToken = default);
}