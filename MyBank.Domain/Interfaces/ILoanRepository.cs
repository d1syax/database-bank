using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ILoanRepository
{
    Task<LoanEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<LoanEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<LoanEntity?> GetByIdWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default);
    
    void Add(LoanEntity loan);
    void Update(LoanEntity loan);
}