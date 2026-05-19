using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayOneAPI.Models.Entity;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(6)]
    [Column("employee_id")]
    public string EmployeeId { get; set; } = string.Empty;   // รหัสพนักงาน 6 หลัก

    [Required]
    [MaxLength(100)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;    // ชื่อ

    [Required]
    [MaxLength(100)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;     // นามสกุล

    [Required]
    [Column("start_date")]
    public DateOnly StartDate { get; set; }                  // วันที่เข้างาน

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty; // BCrypt hash

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
