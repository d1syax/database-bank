using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface IAccountRepository
{
    Task<AccountEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AccountEntity> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AccountEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AccountEntity> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

    void Add(AccountEntity account);
    void Update(AccountEntity account);
}