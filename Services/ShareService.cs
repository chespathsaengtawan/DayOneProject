using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Events;
using DayOneAPI.Models.DTOs.Shares;
using DayOneAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace DayOneAPI.Services;

public class ShareService : IShareService
{
    private readonly AppDbContext _db;

    public ShareService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventShareInfo>> GetSharesByEventAsync(Guid userId, Guid eventId)
    {
        var eventExists = await _db.Events
            .AnyAsync(e => e.Id == eventId && e.UserId == userId);
        if (!eventExists) return [];

        return await _db.EventShares
            .Where(s => s.EventId == eventId)
            .Include(s => s.SharedWithUser)
            .OrderBy(s => s.CreatedAt)
            .Select(s => new EventShareInfo
            {
                Id                   = s.Id,
                SharedWithUserId     = s.SharedWithUserId,
                SharedWithEmployeeId = s.SharedWithUser!.EmployeeId,
                SharedWithName       = s.SharedWithUser.FirstName + " " + s.SharedWithUser.LastName,
                SharedAt             = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<EventShareInfo> ShareEventAsync(Guid userId, Guid eventId, CreateShareRequest request)
    {
        _ = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId)
            ?? throw new KeyNotFoundException("ไม่พบ event");

        var targetUser = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == request.EmployeeId)
            ?? throw new KeyNotFoundException($"ไม่พบพนักงานหมายเลข {request.EmployeeId}");

        if (targetUser.Id == userId)
            throw new InvalidOperationException("ไม่สามารถแชร์กับตัวเองได้");

        var alreadyShared = await _db.EventShares
            .AnyAsync(s => s.EventId == eventId && s.SharedWithUserId == targetUser.Id);
        if (alreadyShared)
            throw new InvalidOperationException("แชร์ event นี้กับพนักงานคนนี้แล้ว");

        var share = new EventShare
        {
            EventId          = eventId,
            SharedByUserId   = userId,
            SharedWithUserId = targetUser.Id,
            CreatedAt        = DateTime.UtcNow
        };

        _db.EventShares.Add(share);
        await _db.SaveChangesAsync();

        return new EventShareInfo
        {
            Id                   = share.Id,
            SharedWithUserId     = targetUser.Id,
            SharedWithEmployeeId = targetUser.EmployeeId,
            SharedWithName       = $"{targetUser.FirstName} {targetUser.LastName}",
            SharedAt             = share.CreatedAt
        };
    }

    public async Task<bool> RemoveShareAsync(Guid userId, Guid eventId, Guid sharedWithUserId)
    {
        var share = await _db.EventShares
            .FirstOrDefaultAsync(s => s.EventId == eventId
                && s.SharedByUserId == userId
                && s.SharedWithUserId == sharedWithUserId);

        if (share is null) return false;

        _db.EventShares.Remove(share);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<SharedEventResponse>> GetSharedWithMeAsync(Guid userId)
    {
        return await _db.EventShares
            .Where(s => s.SharedWithUserId == userId)
            .Include(s => s.Event)
            .ThenInclude(e => e!.EventCategory)
            .Include(s => s.SharedByUser)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SharedEventResponse
            {
                ShareId            = s.Id,
                SharedByEmployeeId = s.SharedByUser!.EmployeeId,
                SharedByName       = s.SharedByUser.FirstName + " " + s.SharedByUser.LastName,
                SharedAt           = s.CreatedAt,
                Event = new EventResponse
                {
                    Id              = s.Event!.Id,
                    UserId          = s.Event.UserId,
                    OwnerFirstName  = s.SharedByUser!.FirstName,
                    OwnerLastName   = s.SharedByUser!.LastName,
                    Title           = s.Event.Title,
                    Description     = s.Event.Description,
                    EventDate       = s.Event.EventDate,
                    StartTime       = s.Event.StartTime,
                    EndTime         = s.Event.EndTime,
                    IsAllDay        = s.Event.IsAllDay,
                    EventCategoryId = s.Event.EventCategoryId,
                    Category        = s.Event.EventCategory != null ? s.Event.EventCategory.Name : null,
                    CategoryColor   = s.Event.EventCategory != null ? s.Event.EventCategory.Color : null,
                    CreatedAt       = s.Event.CreatedAt,
                    UpdatedAt       = s.Event.UpdatedAt
                }
            })
            .ToListAsync();
    }
}
