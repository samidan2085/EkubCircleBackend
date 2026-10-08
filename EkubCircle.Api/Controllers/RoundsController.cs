using EkubCircle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkubCircle.Api.Controllers;

[ApiController]
[Route("api/rounds")]
[Authorize]
public class RoundsController : ControllerBase
{
    private readonly ILotteryService _lotteryService;

    public RoundsController(
        ILotteryService lotteryService)
    {
        _lotteryService = lotteryService;
    }

    [HttpGet("{circleId:guid}")]
    public async Task<IActionResult> Current(
        Guid circleId)
    {
        try
        {
            var result =
                await _lotteryService
                    .GetCurrentRoundAsync(circleId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("{circleId:guid}/complete")]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> Complete(
        Guid circleId)
    {
        try
        {
            var result =
                await _lotteryService
                    .CompleteRoundAsync(circleId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}