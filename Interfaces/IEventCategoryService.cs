using DayOneAPI.Models.DTOs.EventCategories;

namespace DayOneAPI.Interfaces;

public interface IEventCategoryService
{
    Task<List<EventCategoryResponse>> GetAllAsync(bool activeOnly = false);
    Task<EventCategoryResponse?> GetByIdAsync(Guid id);
    Task<EventCategoryResponse> CreateAsync(CreateEventCategoryRequest request);
    Task<EventCategoryResponse?> UpdateAsync(Guid id, UpdateEventCategoryRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<EventCategoryResponse?> ToggleActiveAsync(Guid id);
}
