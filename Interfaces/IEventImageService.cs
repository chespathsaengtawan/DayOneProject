using DayOneAPI.Models.DTOs.Images;
using Microsoft.AspNetCore.Http;

namespace DayOneAPI.Interfaces;

public interface IEventImageService
{
    Task<List<ImageResponse>> GetByEventAsync(Guid userId, Guid eventId);
    Task<ImageResponse> UploadAsync(Guid userId, Guid eventId, IFormFile file);
    Task<bool> DeleteAsync(Guid userId, Guid imageId);
}
