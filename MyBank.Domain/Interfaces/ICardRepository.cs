using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface ICardRepository
{
    Task<CardEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<CardEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task AddAsync(CardEntity card, CancellationToken ct = default);
    Task UpdateAsync(CardEntity card, CancellationToken ct = default);
    Task DeleteAsync(CardEntity card, CancellationToken ct = default);
}