using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<TransactionEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<TransactionEntity>> GetByAccountIdAsync
    (
        Guid accountId, 
        int skip, 
        int take, 
        CancellationToken ct = default
    );

    Task AddAsync(TransactionEntity transaction, CancellationToken ct = default);
    Task UpdateAsync(TransactionEntity transaction, CancellationToken ct = default);
    
    Task<List<TransactionEntity>> GetByAccountIdAndDateRangeAsync
    (
        Guid accountId, 
        DateTime startDate, 
        DateTime endDate,
        int skip,
        int take,
        CancellationToken ct = default
    );
}