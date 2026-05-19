using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Images;
using DayOneAPI.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DayOneAPI.Services;

public class EventImageService : IEventImageService
{
    private static readonly string[] AllowedMimeTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly AppDbContext _db;
    private readonly IFileStorageService _storage;

    public EventImageService(AppDbContext db, IFileStorageService storage)
    {
        _db      = db;
        _storage = storage;
    }

    public async Task<List<ImageResponse>> GetByEventAsync(Guid userId, Guid eventId)
    {
        // Allow access if owner or event is shared with this user
        var hasAccess = await _db.Events.AnyAsync(e => e.Id == eventId && e.UserId == userId)
            || await _db.EventShares.AnyAsync(s => s.EventId == eventId && s.SharedWithUserId == userId);

        if (!hasAccess) return [];

        return await _db.EventImages
            .Where(i => i.EventId == eventId)
            .OrderBy(i => i.CreatedAt)
            .Select(i => ToResponse(i))
            .ToListAsync();
    }

    public async Task<ImageResponse> UploadAsync(Guid userId, Guid eventId, IFormFile file)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("ไฟล์ว่างเปล่า");

        if (file.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("ไฟล์ต้องมีขนาดไม่เกิน 5 MB");

        if (!AllowedMimeTypes.Contains(file.ContentType))
            throw new InvalidOperationException("รองรับเฉพาะไฟล์รูปภาพ (JPEG, PNG, GIF, WebP)");

        _ = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.UserId == userId)
            ?? throw new KeyNotFoundException("ไม่พบ event");

        var uniqueName  = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var folder      = $"{userId}/{eventId}";

        using var stream = file.OpenReadStream();
        var (url, storagePath) = await _storage.UploadAsync(stream, uniqueName, file.ContentType, folder);

        var image = new EventImage
        {
            EventId       = eventId,
            UserId        = userId,
            FileName      = file.FileName,
            FileUrl       = url,
            StoragePath   = storagePath,
            FileSizeBytes = file.Length,
            MimeType      = file.ContentType,
            CreatedAt     = DateTime.UtcNow
        };

        _db.EventImages.Add(image);
        await _db.SaveChangesAsync();

        return ToResponse(image);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid imageId)
    {
        var image = await _db.EventImages
            .FirstOrDefaultAsync(i => i.Id == imageId && i.UserId == userId);

        if (image is null) return false;

        await _storage.DeleteAsync(image.StoragePath);
        _db.EventImages.Remove(image);
        await _db.SaveChangesAsync();

        return true;
    }

    private static ImageResponse ToResponse(EventImage i) => new()
    {
        Id            = i.Id,
        EventId       = i.EventId,
        FileName      = i.FileName,
        FileUrl       = i.FileUrl,
        FileSizeBytes = i.FileSizeBytes,
        MimeType      = i.MimeType,
        CreatedAt     = i.CreatedAt
    };
}
