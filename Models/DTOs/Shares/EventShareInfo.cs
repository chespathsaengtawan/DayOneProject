namespace DayOneAPI.Models.DTOs.Shares;

public class EventShareInfo
{
    public Guid Id { get; set; }
    public Guid SharedWithUserId { get; set; }
    public string SharedWithEmployeeId { get; set; } = string.Empty;
    public string SharedWithName { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
}
