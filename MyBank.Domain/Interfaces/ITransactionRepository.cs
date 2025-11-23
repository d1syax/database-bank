using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<TransactionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TransactionEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<TransactionEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<TransactionEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(TransactionEntity transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(TransactionEntity transaction, CancellationToken cancellationToken = default);
}