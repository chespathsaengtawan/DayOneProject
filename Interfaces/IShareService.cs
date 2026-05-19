using DayOneAPI.Models.DTOs.Shares;

namespace DayOneAPI.Interfaces;

public interface IShareService
{
    Task<List<EventShareInfo>> GetSharesByEventAsync(Guid userId, Guid eventId);
    Task<EventShareInfo> ShareEventAsync(Guid userId, Guid eventId, CreateShareRequest request);
    Task<bool> RemoveShareAsync(Guid userId, Guid eventId, Guid sharedWithUserId);
    Task<List<SharedEventResponse>> GetSharedWithMeAsync(Guid userId);
}
