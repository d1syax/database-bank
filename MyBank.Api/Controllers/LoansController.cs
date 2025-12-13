using Microsoft.AspNetCore.Mvc;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.Services;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LoanService _loanService;

    public LoansController(LoanService loanService)
    {
        _loanService = loanService;
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserLoans(Guid userId, CancellationToken ct)
    {
        var loans = await _loanService.GetUserLoansAsync(userId, ct);
        return Ok(loans);
    }

    [HttpPost("loan")]
    public async Task<IActionResult> IssueLoan(CreateLoanRequest request, CancellationToken ct)
    {
        var result = await _loanService.IssueLoanAsync(request, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("repay")]
    public async Task<IActionResult> RepayLoan(RepayLoanRequest request, CancellationToken ct)
    {
        var result = await _loanService.RepayLoanAsync(request, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok("Payment processed successfully");
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoan(Guid id, CancellationToken ct)
    {
        var result = await _loanService.ArchiveLoanAsync(id, ct);
        if (result.IsFailure) return BadRequest(result.Error);
        return NoContent();
    }
}