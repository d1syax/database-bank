namespace MyBank.Application.Services;

using CSharpFunctionalExtensions;
using MyBank.Api.DTOs;
using MyBank.Api.DTOs.Responses;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;

public class CardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CardService(ICardRepository cardRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<CardResponse>> GetCardsByAccountAsync(Guid accountId, CancellationToken ct)
    {
        var cards = await _cardRepository.GetByAccountIdAsync(accountId, ct);
        
        var response = new List<CardResponse>();
        foreach (var c in cards)
        {
            response.Add(new CardResponse(
                c.Id, c.CardNumber, c.CardType.ToString(), 
                c.ExpirationDate, c.Status.ToString(), c.DailyLimit
            ));
        }
        return response;
    }

    public async Task<Result<CardResponse>> IssueCardAsync(CreateCardRequest request, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account == null) 
            return Result.Failure<CardResponse>("Account not found");

        var cardResult = CardEntity.Create(account.Id, request.CardType);
        if (cardResult.IsFailure) 
            return Result.Failure<CardResponse>(cardResult.Error);

        var card = cardResult.Value;
        await _cardRepository.AddAsync(card, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CardResponse(
            card.Id, card.CardNumber, card.CardType.ToString(), 
            card.ExpirationDate, card.Status.ToString(), card.DailyLimit
        ));
    }

    public async Task<Result> BlockCardAsync(Guid cardId, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(cardId, ct);
        if (card == null) 
            return Result.Failure("Card not found");

        var result = card.Block();
        if (result.IsFailure) 
            return result;

        await _cardRepository.UpdateAsync(card, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
    
    public async Task<Result> UpdateCardLimitAsync(Guid cardId, decimal newLimit, CancellationToken ct)
    {
        var card = await _cardRepository.GetByIdAsync(cardId, ct);
        if (card == null) 
            return Result.Failure("Card not found");

        var result = card.ChangeLimit(newLimit);
        if (result.IsFailure) 
            return result;

        await _cardRepository.UpdateAsync(card, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

}