using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOneAPI.Models.Entity;

[Table("event_shares")]
public class EventShare
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid SharedByUserId { get; set; }

    [Required]
    public Guid SharedWithUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(SharedByUserId))]
    public User? SharedByUser { get; set; }

    [ForeignKey(nameof(SharedWithUserId))]
    public User? SharedWithUser { get; set; }
}
