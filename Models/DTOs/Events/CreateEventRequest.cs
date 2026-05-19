using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.Events;

public class CreateEventRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateOnly EventDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsAllDay { get; set; } = false;

    [MaxLength(50)]
    public string? Category { get; set; }
}
