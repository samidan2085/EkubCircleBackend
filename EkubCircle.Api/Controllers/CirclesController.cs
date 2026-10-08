using System.Security.Claims;
using EkubCircle.Application.DTOs.Circle;
using EkubCircle.Application.DTOs.Member;
using EkubCircle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EkubCircle.Application.DTOs.Round;

namespace EkubCircle.Api.Controllers;
[Authorize] 
[ApiController]
[Route("api/circles")]
// Enabled authorization for the whole controller

public class CirclesController : ControllerBase
{
    private readonly ICircleService _circleService;

    public CirclesController(ICircleService circleService)
    {
        _circleService = circleService;
    }
    [HttpPost]
[Authorize]
public async Task<IActionResult> Create( CircleDetailDto request)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
if(string.IsNullOrWhiteSpace(userId))
        return Unauthorized();
    
    var result = await _circleService.CreateCircleAsync(request);

    return Created("", result);
}
    [HttpGet]
    [HttpGet]
public IActionResult GetAll()
{
    return Ok(new
    {
        Message = "GET /api/circles works",
        IsAuthenticated = User.Identity?.IsAuthenticated,
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
        Email = User.FindFirstValue(ClaimTypes.Email)
    });
}

    [HttpGet("{circleId:guid}")]
    public async Task<IActionResult> Get(Guid circleId)
    {
        var result = await _circleService.GetCircleAsync(circleId);
        return Ok(result);
    }

    [HttpPost("{circleId:guid}/members")]
    [Authorize(Roles = "Organizer")] // Enforce organizer role if applicable
    public async Task<IActionResult> AddMember(Guid circleId, AddMemberDto request)
    {
        var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(organizerId))
            return Unauthorized();

        await _circleService.AddMemberAsync(circleId, request, organizerId);

        return Ok(new
        {
            message = "Member created successfully."
        });
    }

    [HttpGet("{circleId:guid}/members")]
    public async Task<IActionResult> Members(Guid circleId)
    {
        var result = await _circleService.GetMembersAsync(circleId);
        return Ok(result);
    }
    [HttpGet("test-auth")]
[Authorize]
public IActionResult TestAuth()
{
    return Ok(new
    {
        IsAuthenticated = User.Identity?.IsAuthenticated,
        UserId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
        Email = User.FindFirst(
            System.Security.Claims.ClaimTypes.Email)?.Value,
        Name = User.Identity?.Name
    });
}
[HttpGet("my")]
[Authorize(Roles = "Organizer")]
public async Task<IActionResult> GetMyCircle()
{
    var organizerId =
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(organizerId))
        return Unauthorized();

    var circle =
        await _circleService.GetMyCircleDetailsAsync(organizerId);

    if (circle == null)
    {
        return NotFound(new
        {
            message = "You do not have a circle yet."
        });
    }

    return Ok(circle);
}
[HttpPost("{circleId:guid}/start")]
[Authorize(Roles = "Organizer")]
public async Task<IActionResult> StartCircle(Guid circleId)
{
    var organizerId =
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(organizerId))
    {
        return Unauthorized();
    }

    try
    {
        await _circleService.StartCircleAsync(
            circleId,
            organizerId);

        return Ok(new
        {
            message =
                "Circle started successfully. Member list is now locked and rounds have been created."
        });
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
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}
[HttpPost("rounds/toggle-payment")]
[Authorize(Roles = "Organizer")]
public async Task<IActionResult> TogglePayment(
    TogglePaymentDto request)
{
    var organizerId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(organizerId))
    {
        return Unauthorized();
    }

    try
    {
        await _circleService.TogglePaymentAsync(
            request.CircleId,
            request.RoundNumber,
            request.MemberId,
            organizerId);

        return Ok(new
        {
            message =
                "Member payment recorded successfully."
        });
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
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}
}