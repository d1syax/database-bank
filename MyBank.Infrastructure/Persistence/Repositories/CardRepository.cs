using Microsoft.EntityFrameworkCore;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistence;

namespace MyBank.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    public CardRepository(BankDbContext context)
    {
        _context = context;
    }

    private readonly BankDbContext _context;
    
    public async Task<CardEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Cards.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CardEntity?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cards.FirstOrDefaultAsync(x => x.CardNumber == cardNumber, cancellationToken);
    }

    public async Task<List<CardEntity>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards.Where(x => x.AccountId == accountId).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cards.AnyAsync(x => x.CardNumber == cardNumber);
    }

    public async Task AddAsync(CardEntity card, CancellationToken cancellationToken = default)
    {
        await _context.Cards.AddAsync(card, cancellationToken);
    }

    public Task UpdateAsync(CardEntity card, CancellationToken cancellationToken = default)
    {
        _context.Cards.Update(card);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CardEntity card, CancellationToken cancellationToken = default)
    {
        card.IsDeleted = true;
        card.DeletedAt = DateTime.UtcNow;
        _context.Cards.Update(card);
        return Task.CompletedTask;
    }
}