using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<TransactionEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TransactionEntity>> GetByAccountIdAsync(Guid accountId, int limit = 20, CancellationToken cancellationToken = default);

    void Add(TransactionEntity transaction);
}