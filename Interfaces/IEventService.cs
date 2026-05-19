using DayOneAPI.Models.DTOs.Events;

namespace DayOneAPI.Interfaces;

public interface IEventService
{
    Task<List<EventResponse>> GetByDateAsync(Guid userId, DateOnly date);
    Task<List<EventResponse>> GetByMonthAsync(Guid userId, int year, int month);
    Task<EventResponse?> GetByIdAsync(Guid userId, Guid eventId);
    Task<EventResponse> CreateAsync(Guid userId, CreateEventRequest request);
    Task<EventResponse?> UpdateAsync(Guid userId, Guid eventId, UpdateEventRequest request);
    Task<bool> DeleteAsync(Guid userId, Guid eventId);
}
