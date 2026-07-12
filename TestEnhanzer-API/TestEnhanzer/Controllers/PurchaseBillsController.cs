using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestEnhanzer.Models.Dtos;
using TestEnhanzer.Services;

namespace TestEnhanzer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseBillsController : ControllerBase
{
    private readonly IPurchaseBillCalculator _calculator;

    public PurchaseBillsController(IPurchaseBillCalculator calculator)
    {
        _calculator = calculator;
    }

    /// <summary>Validates and calculates totals for a single purchase bill line.</summary>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(PurchaseBillLineResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Calculate([FromBody] PurchaseBillLineDto line)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        return Ok(_calculator.CalculateLine(line));
    }

    /// <summary>Accepts the full set of lines and returns each calculated line plus a summary.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Submit([FromBody] List<PurchaseBillLineDto> lines)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (lines.Count == 0)
        {
            return BadRequest(new { message = "At least one purchase bill line is required." });
        }

        var calculated = lines.Select(_calculator.CalculateLine).ToList();
        var summary = _calculator.Summarize(lines);

        return Ok(new { lines = calculated, summary });
    }
}
