using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "กรุณากรอกรหัสพนักงาน")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "รหัสพนักงานต้องเป็นตัวเลข 6 หลัก")]
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>วันที่เข้างานในรูปแบบ ddMMyyyy เช่น 15032020 = 15 มีนาคม 2563</summary>
    [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน (วันที่เข้างาน รูปแบบ ddMMyyyy)")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "รหัสผ่านต้องเป็นตัวเลข 8 หลัก (ddMMyyyy)")]
    public string Password { get; set; } = string.Empty;
}
