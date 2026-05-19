using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "กรุณากรอกรหัสพนักงาน")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "รหัสพนักงานต้องเป็นตัวเลข 6 หลัก")]
    public string EmployeeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกชื่อ")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกวันที่เข้างาน")]
    public DateOnly StartDate { get; set; }
}
