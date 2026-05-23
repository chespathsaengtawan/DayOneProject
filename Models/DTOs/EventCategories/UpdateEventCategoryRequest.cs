using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.EventCategories;

public class UpdateEventCategoryRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(7)]
    public string? Color { get; set; }

    public bool? IsActive { get; set; }
}
