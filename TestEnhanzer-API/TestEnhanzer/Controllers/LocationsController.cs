using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestEnhanzer.Models.Dtos;
using TestEnhanzer.Services;

namespace TestEnhanzer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<LocationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocations(CancellationToken ct)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? string.Empty;
        var locations = await _locationService.GetLocationsAsync(username, ct);
        return Ok(locations);
    }
}
