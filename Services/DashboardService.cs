using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Dashboard;
using DayOneAPI.Models.DTOs.Events;
using Microsoft.EntityFrameworkCore;

namespace DayOneAPI.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardResponse> GetDashboardAsync(Guid userId)
    {
        var today      = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekEnd    = today.AddDays(7);
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var monthEnd   = monthStart.AddMonths(1).AddDays(-1);

        var todayCount = await _db.Events
            .CountAsync(e => e.UserId == userId && e.EventDate == today);

        var thisWeekCount = await _db.Events
            .CountAsync(e => e.UserId == userId && e.EventDate >= today && e.EventDate <= weekEnd);

        var thisMonthCount = await _db.Events
            .CountAsync(e => e.UserId == userId && e.EventDate >= monthStart && e.EventDate <= monthEnd);

        var upcomingEvents = await _db.Events
            .Where(e => e.UserId == userId && e.EventDate >= today && e.EventDate <= weekEnd)
            .OrderBy(e => e.EventDate)
            .ThenBy(e => e.IsAllDay ? 0 : 1)
            .ThenBy(e => e.StartTime)
            .Take(10)
            .Select(e => new EventResponse
            {
                Id          = e.Id,
                UserId      = e.UserId,
                Title       = e.Title,
                Description = e.Description,
                EventDate   = e.EventDate,
                StartTime   = e.StartTime,
                EndTime     = e.EndTime,
                IsAllDay    = e.IsAllDay,
                Category    = e.Category,
                CreatedAt   = e.CreatedAt,
                UpdatedAt   = e.UpdatedAt
            })
            .ToListAsync();

        var eventsByCategory = await _db.Events
            .Where(e => e.UserId == userId && e.EventDate >= monthStart && e.EventDate <= monthEnd)
            .GroupBy(e => e.Category ?? "ไม่ระบุ")
            .Select(g => new CategoryCount { Category = g.Key, Count = g.Count() })
            .OrderByDescending(c => c.Count)
            .ToListAsync();

        var recentlyAdded = await _db.Events
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .Select(e => new EventResponse
            {
                Id          = e.Id,
                UserId      = e.UserId,
                Title       = e.Title,
                Description = e.Description,
                EventDate   = e.EventDate,
                StartTime   = e.StartTime,
                EndTime     = e.EndTime,
                IsAllDay    = e.IsAllDay,
                Category    = e.Category,
                CreatedAt   = e.CreatedAt,
                UpdatedAt   = e.UpdatedAt
            })
            .ToListAsync();

        return new DashboardResponse
        {
            TodayCount       = todayCount,
            ThisWeekCount    = thisWeekCount,
            ThisMonthCount   = thisMonthCount,
            UpcomingEvents   = upcomingEvents,
            EventsByCategory = eventsByCategory,
            RecentlyAdded    = recentlyAdded
        };
    }
}
