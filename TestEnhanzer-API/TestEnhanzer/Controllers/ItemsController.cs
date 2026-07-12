using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestEnhanzer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private static readonly string[] Items =
    {
        "Mango", "Apple", "Banana", "Orange", "Grapes", "Kiwi", "Strawberry"
    };

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetItems([FromQuery] string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Ok(Items);
        }

        var filtered = Items
            .Where(i => i.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return Ok(filtered);
    }
}
