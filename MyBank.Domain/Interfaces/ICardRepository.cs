using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ICardRepository
{
    Task<CardEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CardEntity> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
    Task<List<CardEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    void Add(CardEntity card);
    void Update(CardEntity card);
}