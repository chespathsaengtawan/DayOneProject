using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.Events;

public class UpdateEventRequest
{
    [MaxLength(200)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateOnly? EventDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool? IsAllDay { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }
}
