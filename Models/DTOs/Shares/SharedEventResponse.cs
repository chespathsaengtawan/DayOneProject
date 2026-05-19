using DayOneAPI.Models.DTOs.Events;

namespace DayOneAPI.Models.DTOs.Shares;

public class SharedEventResponse
{
    public Guid ShareId { get; set; }
    public string SharedByEmployeeId { get; set; } = string.Empty;
    public string SharedByName { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
    public EventResponse Event { get; set; } = null!;
}
