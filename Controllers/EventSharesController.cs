using System.Security.Claims;
using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DayOneAPI.Controllers;

[ApiController]
[Authorize]
public class EventSharesController : ControllerBase
{
    private readonly IShareService _shareService;

    public EventSharesController(IShareService shareService)
    {
        _shareService = shareService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/events/{eventId}/shares
    [HttpGet("events/{eventId:guid}/shares")]
    public async Task<IActionResult> GetShares(Guid eventId)
    {
        var shares = await _shareService.GetSharesByEventAsync(GetUserId(), eventId);
        return Ok(shares);
    }

    // POST /api/events/{eventId}/shares
    [HttpPost("events/{eventId:guid}/shares")]
    public async Task<IActionResult> ShareEvent(Guid eventId, [FromBody] CreateShareRequest request)
    {
        try
        {
            var share = await _shareService.ShareEventAsync(GetUserId(), eventId, request);
            return Ok(share);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE /api/events/{eventId}/shares/{sharedWithUserId}
    [HttpDelete("events/{eventId:guid}/shares/{sharedWithUserId:guid}")]
    public async Task<IActionResult> RemoveShare(Guid eventId, Guid sharedWithUserId)
    {
        var removed = await _shareService.RemoveShareAsync(GetUserId(), eventId, sharedWithUserId);
        if (!removed) return NotFound(new { message = "ไม่พบการแชร์" });
        return NoContent();
    }

    // GET /api/events/shared-with-me
    [HttpGet("events/shared-with-me")]
    public async Task<IActionResult> GetSharedWithMe()
    {
        var events = await _shareService.GetSharedWithMeAsync(GetUserId());
        return Ok(events);
    }
}
