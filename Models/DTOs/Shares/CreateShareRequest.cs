using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.Shares;

public class CreateShareRequest
{
    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Employee ID ต้องเป็นตัวเลข 6 หลัก")]
    public string EmployeeId { get; set; } = string.Empty;
}
