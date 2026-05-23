using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Events;
using DayOneAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace DayOneAPI.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _db;

    public EventService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventResponse>> GetByDateAsync(Guid userId, DateOnly date)
    {
        var events = await _db.Events
            .Include(e => e.User)
            .Include(e => e.EventCategory)
            .Where(e => e.UserId == userId && e.EventDate == date)
            .OrderBy(e => e.IsAllDay ? 0 : 1)
            .ThenBy(e => e.StartTime)
            .ToListAsync();

        return events.Select(e => ToResponse(e, e.User!)).ToList();
    }

    public async Task<List<EventResponse>> GetByMonthAsync(Guid userId, int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var events = await _db.Events
            .Include(e => e.User)
            .Include(e => e.EventCategory)
            .Where(e => e.UserId == userId && e.EventDate >= start && e.EventDate <= end)
            .OrderBy(e => e.EventDate)
            .ThenBy(e => e.IsAllDay ? 0 : 1)
            .ThenBy(e => e.StartTime)
            .ToListAsync();

        return events.Select(e => ToResponse(e, e.User!)).ToList();
    }

    public async Task<EventResponse?> GetByIdAsync(Guid userId, Guid eventId)
    {
        var ev = await _db.Events
            .Include(e => e.User)
            .Include(e => e.EventCategory)
            .FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId);

        return ev is null ? null : ToResponse(ev, ev.User!);
    }

    public async Task<EventResponse> CreateAsync(Guid userId, CreateEventRequest request)
    {
        var ev = new Event
        {
            UserId            = userId,
            Title             = request.Title,
            Description       = request.Description,
            EventDate         = request.EventDate,
            StartTime         = request.StartTime,
            EndTime           = request.EndTime,
            IsAllDay          = request.IsAllDay,
            EventCategoryId   = request.EventCategoryId,
            CreatedAt         = DateTime.UtcNow,
            UpdatedAt         = DateTime.UtcNow
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        await _db.Entry(ev).Reference(e => e.User).LoadAsync();
        await _db.Entry(ev).Reference(e => e.EventCategory).LoadAsync();

        return ToResponse(ev, ev.User!);
    }

    public async Task<EventResponse?> UpdateAsync(Guid userId, Guid eventId, UpdateEventRequest request)
    {
        var ev = await _db.Events
            .FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId);

        if (ev is null) return null;

        if (request.Title is not null)                ev.Title           = request.Title;
        if (request.Description is not null)           ev.Description     = request.Description;
        if (request.EventDate.HasValue)                ev.EventDate       = request.EventDate.Value;
        if (request.StartTime.HasValue)                ev.StartTime       = request.StartTime;
        if (request.EndTime.HasValue)                  ev.EndTime         = request.EndTime;
        if (request.IsAllDay.HasValue)                 ev.IsAllDay        = request.IsAllDay.Value;
        if (request.EventCategoryId.HasValue)          ev.EventCategoryId = request.EventCategoryId;
        ev.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(ev).Reference(e => e.User).LoadAsync();
        await _db.Entry(ev).Reference(e => e.EventCategory).LoadAsync();

        return ToResponse(ev, ev.User!);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid eventId)
    {
        var ev = await _db.Events
            .FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId);

        if (ev is null) return false;

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();

        return true;
    }

    private static EventResponse ToResponse(Event ev, User user) => new()
    {
        Id              = ev.Id,
        UserId          = ev.UserId,
        OwnerFirstName  = user.FirstName,
        OwnerLastName   = user.LastName,
        Title           = ev.Title,
        Description     = ev.Description,
        EventDate       = ev.EventDate,
        StartTime       = ev.StartTime,
        EndTime         = ev.EndTime,
        IsAllDay        = ev.IsAllDay,
        EventCategoryId = ev.EventCategoryId,
        Category        = ev.EventCategory?.Name,
        CreatedAt       = ev.CreatedAt,
        UpdatedAt       = ev.UpdatedAt
    };
}
