using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Feed;
using Microsoft.EntityFrameworkCore;

namespace DayOneAPI.Services;

public class FeedService : IFeedService
{
    private readonly AppDbContext _db;

    public FeedService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<FeedResponse> GetFeedAsync(FeedQuery query)
    {
        var q = _db.Events
            .Include(e => e.User)
            .Include(e => e.EventCategory)
            .AsQueryable();

        // --- Filters ---
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var keyword = query.Search.Trim().ToLower();
            q = q.Where(e =>
                e.Title.ToLower().Contains(keyword) ||
                (e.Description != null && e.Description.ToLower().Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.EmployeeId))
            q = q.Where(e => e.User!.EmployeeId == query.EmployeeId.Trim());

        if (!string.IsNullOrWhiteSpace(query.OwnerName))
        {
            var name = query.OwnerName.Trim().ToLower();
            q = q.Where(e =>
                (e.User!.FirstName + " " + e.User.LastName).ToLower().Contains(name));
        }

        if (query.CategoryId.HasValue)
            q = q.Where(e => e.EventCategoryId == query.CategoryId.Value);

        if (query.DateFrom.HasValue)
            q = q.Where(e => e.EventDate >= query.DateFrom.Value);

        if (query.DateTo.HasValue)
            q = q.Where(e => e.EventDate <= query.DateTo.Value);

        // --- Sort ---
        q = query.Sort switch
        {
            "oldest"     => q.OrderBy(e => e.CreatedAt),
            "event_asc"  => q.OrderBy(e => e.EventDate).ThenBy(e => e.StartTime),
            "event_desc" => q.OrderByDescending(e => e.EventDate).ThenByDescending(e => e.StartTime),
            _            => q.OrderByDescending(e => e.CreatedAt) // newest (default)
        };

        // --- Pagination ---
        var totalCount = await q.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => new FeedEventResponse
            {
                Id              = e.Id,
                Title           = e.Title,
                Description     = e.Description,
                EventDate       = e.EventDate,
                StartTime       = e.StartTime,
                EndTime         = e.EndTime,
                IsAllDay        = e.IsAllDay,
                EventCategoryId = e.EventCategoryId,
                CategoryName    = e.EventCategory != null ? e.EventCategory.Name : null,
                OwnerId         = e.UserId,
                OwnerEmployeeId = e.User!.EmployeeId,
                OwnerFirstName  = e.User.FirstName,
                OwnerLastName   = e.User.LastName,
                CreatedAt       = e.CreatedAt,
                UpdatedAt       = e.UpdatedAt
            })
            .ToListAsync();

        return new FeedResponse
        {
            Items           = items,
            Page            = query.Page,
            PageSize        = query.PageSize,
            TotalCount      = totalCount,
            TotalPages      = totalPages,
            HasNextPage     = query.Page < totalPages,
            HasPreviousPage = query.Page > 1
        };
    }
}
