using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface IAccountRepository
{
    Task<AccountEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<AccountEntity>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<AccountEntity?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default);
    Task AddAsync(AccountEntity account, CancellationToken ct = default);
    Task UpdateAsync(AccountEntity account, CancellationToken ct = default);
    Task DeleteAsync(AccountEntity account, CancellationToken ct = default);
}