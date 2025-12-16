using Microsoft.AspNetCore.Mvc;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.Services;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;
    public TransactionsController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(CreateTransferRequest request, CancellationToken ct)
    {
        var result = await _transactionService.TransferAsync(request, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetHistory(Guid accountId, CancellationToken ct)
    {
        var history = await _transactionService.GetAccountHistoryAsync(accountId, ct);
        return Ok(history);
    }
    
    [HttpPost("history/range")]
    public async Task<IActionResult> GetHistoryByDateRange(TransactionHistoryRequest request, CancellationToken ct)
    {
        var history = await _transactionService.GetAccountHistoryByDateRangeAsync(
            request.AccountId, 
            request.StartDate, 
            request.EndDate, 
            ct);
        return Ok(history);
    }
}