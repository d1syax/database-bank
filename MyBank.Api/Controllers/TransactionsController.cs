using Microsoft.AspNetCore.Mvc;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.Services;
using MyBank.Application.DTOs.Common;
    
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
    public async Task<IActionResult> GetHistory
    (
        Guid accountId, 
        [FromQuery] PagedRequest pagination,
        CancellationToken ct
    )
    {
        var history = await _transactionService.GetAccountHistoryAsync(accountId, pagination, ct);
        return Ok(history);
    }
    
    [HttpPost("history/range")]
    public async Task<IActionResult> GetHistoryByDateRange
    (
        [FromBody] TransactionHistoryRequest request,
        [FromQuery] PagedRequest pagination,
        CancellationToken ct
    )
    {
        var history = await _transactionService.GetAccountHistoryByDateRangeAsync
        (
            request.AccountId, 
            request.StartDate, 
            request.EndDate, 
            pagination,
            ct
        );
        return Ok(history);
    }
}