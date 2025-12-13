using Microsoft.AspNetCore.Mvc;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.Services;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountsController(AccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId, CancellationToken ct)
    {
        var accounts = await _accountService.GetUserAccountsAsync(userId, ct);
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequest request, CancellationToken ct)
    {
        var result = await _accountService.CreateAccountAsync(request, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> OpenDepositAccount(CreateDepositRequest request, CancellationToken ct)
    {
        var result = await _accountService.OpenDepositAccountAsync(request, ct);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return Ok(result.Value);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        var result = await _accountService.CloseAccountAsync(id, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok("Account closed successfully");
    }
}