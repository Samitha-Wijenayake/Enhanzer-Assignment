using Microsoft.AspNetCore.Mvc;
using TestEnhanzer.Models.Dtos;
using TestEnhanzer.Services;

namespace TestEnhanzer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var outcome = await _authService.LoginAsync(request, ct);

        if (!outcome.Success)
        {
            return Unauthorized(new { message = outcome.ErrorMessage });
        }

        return Ok(outcome.Data);
    }
}
