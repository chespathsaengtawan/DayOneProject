using System.Security.Claims;
using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DayOneAPI.Controllers;

[ApiController]
[Route("events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/events?date=2026-05-19
    [HttpGet]
    public async Task<IActionResult> GetByDate([FromQuery] DateOnly date)
    {
        var events = await _eventService.GetByDateAsync(GetUserId(), date);
        return Ok(events);
    }

    // GET /api/events/month?year=2026&month=5
    [HttpGet("month")]
    public async Task<IActionResult> GetByMonth([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 1 || month < 1 || month > 12)
            return BadRequest(new { message = "year/month ไม่ถูกต้อง" });

        var events = await _eventService.GetByMonthAsync(GetUserId(), year, month);
        return Ok(events);
    }

    // GET /api/events/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await _eventService.GetByIdAsync(GetUserId(), id);
        if (ev is null) return NotFound(new { message = "ไม่พบ event" });
        return Ok(ev);
    }

    // POST /api/events
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        var ev = await _eventService.CreateAsync(GetUserId(), request);
        return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ev);
    }

    // PUT /api/events/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request)
    {
        var ev = await _eventService.UpdateAsync(GetUserId(), id, request);
        if (ev is null) return NotFound(new { message = "ไม่พบ event" });
        return Ok(ev);
    }

    // DELETE /api/events/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _eventService.DeleteAsync(GetUserId(), id);
        if (!deleted) return NotFound(new { message = "ไม่พบ event" });
        return NoContent();
    }
}
