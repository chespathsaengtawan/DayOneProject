namespace DayOneAPI.Models.DTOs.Images;

public class ImageResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
