using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.EventCategories;

public class CreateEventCategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
