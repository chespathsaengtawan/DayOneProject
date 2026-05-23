namespace DayOneAPI.Models.DTOs.Feed;

public class FeedEventResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly EventDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public bool IsAllDay { get; set; }

    public Guid? EventCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryColor { get; set; }

    public Guid OwnerId { get; set; }
    public string OwnerEmployeeId { get; set; } = string.Empty;
    public string OwnerFirstName { get; set; } = string.Empty;
    public string OwnerLastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
