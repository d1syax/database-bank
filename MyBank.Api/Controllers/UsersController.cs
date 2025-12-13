using Microsoft.AspNetCore.Mvc;
using MyBank.Application.DTOs.Requests;
using MyBank.Application.Services;

namespace MyBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest request, CancellationToken ct)
    {
        var result = await _userService.RegisterAsync(request, ct);
        
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetProfile), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(Guid id, CancellationToken ct)
    {
        var result = await _userService.GetProfileAsync(id, ct);
        
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        var result = await _userService.UpdateProfileAsync(id, request, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        var result = await _userService.DeleteUserAsync(id, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent(); 
    }
}