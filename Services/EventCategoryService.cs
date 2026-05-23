using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.EventCategories;
using DayOneAPI.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace DayOneAPI.Services;

public class EventCategoryService : IEventCategoryService
{
    private readonly AppDbContext _db;

    public EventCategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventCategoryResponse>> GetAllAsync(bool activeOnly = false)
    {
        var query = _db.EventCategories.AsQueryable();

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => ToResponse(c))
            .ToListAsync();
    }

    public async Task<EventCategoryResponse?> GetByIdAsync(Guid id)
    {
        var category = await _db.EventCategories.FindAsync(id);
        return category is null ? null : ToResponse(category);
    }

    public async Task<EventCategoryResponse> CreateAsync(CreateEventCategoryRequest request)
    {
        var duplicate = await _db.EventCategories
            .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());

        if (duplicate)
            throw new InvalidOperationException($"มีหมวดหมู่ '{request.Name}' อยู่แล้ว");

        var category = new EventCategory
        {
            Name      = request.Name.Trim(),
            Color     = request.Color,
            IsActive  = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.EventCategories.Add(category);
        await _db.SaveChangesAsync();

        return ToResponse(category);
    }

    public async Task<EventCategoryResponse?> UpdateAsync(Guid id, UpdateEventCategoryRequest request)
    {
        var category = await _db.EventCategories.FindAsync(id);
        if (category is null) return null;

        if (request.Name is not null)
        {
            var duplicate = await _db.EventCategories
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == request.Name.ToLower());

            if (duplicate)
                throw new InvalidOperationException($"มีหมวดหมู่ '{request.Name}' อยู่แล้ว");

            category.Name = request.Name.Trim();
        }

        if (request.Color is not null)
            category.Color = request.Color;

        if (request.IsActive.HasValue)
            category.IsActive = request.IsActive.Value;

        category.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToResponse(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _db.EventCategories.FindAsync(id);
        if (category is null) return false;

        var inUse = await _db.Events.AnyAsync(e => e.EventCategoryId == id);
        if (inUse)
            throw new InvalidOperationException("ไม่สามารถลบหมวดหมู่ที่มี event ใช้งานอยู่ได้");

        _db.EventCategories.Remove(category);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<EventCategoryResponse?> ToggleActiveAsync(Guid id)
    {
        var category = await _db.EventCategories.FindAsync(id);
        if (category is null) return null;

        category.IsActive  = !category.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToResponse(category);
    }

    private static EventCategoryResponse ToResponse(EventCategory c) => new()
    {
        Id        = c.Id,
        Name      = c.Name,
        Color     = c.Color,
        IsActive  = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
