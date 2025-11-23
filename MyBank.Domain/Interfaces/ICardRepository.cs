using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ICardRepository
{
    Task<CardEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CardEntity?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    Task<List<CardEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    Task AddAsync(CardEntity card, CancellationToken cancellationToken = default);
    Task UpdateAsync(CardEntity card, CancellationToken cancellationToken = default);
    Task DeleteAsync(CardEntity card, CancellationToken cancellationToken = default);

}