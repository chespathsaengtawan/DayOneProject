using System.Security.Claims;
using DayOneAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DayOneAPI.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/images")]
[Authorize]
public class EventImagesController : ControllerBase
{
    private readonly IEventImageService _imageService;

    public EventImagesController(IEventImageService imageService)
    {
        _imageService = imageService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/events/{eventId}/images
    [HttpGet]
    public async Task<IActionResult> GetImages(Guid eventId)
    {
        var images = await _imageService.GetByEventAsync(GetUserId(), eventId);
        return Ok(images);
    }

    // POST /api/events/{eventId}/images
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5_242_880)]
    public async Task<IActionResult> Upload(Guid eventId, IFormFile file)
    {
        try
        {
            var image = await _imageService.UploadAsync(GetUserId(), eventId, file);
            return Ok(image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/events/{eventId}/images/{imageId}
    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid eventId, Guid imageId)
    {
        var deleted = await _imageService.DeleteAsync(GetUserId(), imageId);
        if (!deleted) return NotFound(new { message = "ไม่พบรูปภาพ" });
        return NoContent();
    }
}
