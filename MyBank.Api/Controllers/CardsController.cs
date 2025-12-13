using Microsoft.AspNetCore.Mvc;
using MyBank.Api.DTOs;
using MyBank.Application.Services;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly CardService _cardService;

    public CardsController(CardService cardService)
    {
        _cardService = cardService;
    }
    
    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccount(Guid accountId, CancellationToken ct)
    {
        var cards = await _cardService.GetCardsByAccountAsync(accountId, ct);
        return Ok(cards);
    }

    [HttpPost("issue")]
    public async Task<IActionResult> IssueCard(CreateCardRequest request, CancellationToken ct)
    {
        var result = await _cardService.IssueCardAsync(request, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id}/block")]
    public async Task<IActionResult> BlockCard(Guid id, CancellationToken ct)
    {
        var result = await _cardService.BlockCardAsync(id, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok("Card blocked");
    }
    
    [HttpPut("{id}/limit")]
    public async Task<IActionResult> UpdateCardLimit(Guid id, UpdateCardLimitRequest request, CancellationToken ct)
    {
        var result = await _cardService.UpdateCardLimitAsync(id, request.NewDailyLimit, ct);
    
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok("Card limit updated successfully");
    }
}