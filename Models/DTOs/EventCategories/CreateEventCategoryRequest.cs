using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.EventCategories;

public class CreateEventCategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(7)]
    public string Color { get; set; } = "#3B82F6";
}
