using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ILoanRepository
{
    Task<LoanEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<LoanEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<LoanEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task AddAsync(LoanEntity loan, CancellationToken ct = default);
    Task UpdateAsync(LoanEntity loan, CancellationToken ct = default);
    Task DeleteAsync(LoanEntity loan, CancellationToken ct = default);
}