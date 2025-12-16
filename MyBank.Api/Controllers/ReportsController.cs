using Microsoft.AspNetCore.Mvc;
using MyBank.Application.Services;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }
    
    private readonly ReportService _reportService;
    
    [HttpGet("report")]
    public async Task<IActionResult> GetLoanReport(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetUserLoanReportAsync(cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }
    
    [HttpGet("monthly-transactions/{userId}")]
    public async Task<IActionResult> GetMonthlyTransactionReport(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetMonthlyTransactionReportAsync(userId, cancellationToken);
    
        if (result.IsFailure)
            return BadRequest(result.Error);
    
        return Ok(result.Value);
    }
}