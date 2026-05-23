using System.ComponentModel.DataAnnotations;

namespace DayOneAPI.Models.DTOs.Feed;

public class FeedQuery
{
    private int _page = 1;
    private int _pageSize = 10;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : value > 50 ? 50 : value;
    }

    /// <summary>newest | oldest | event_asc | event_desc</summary>
    public string Sort { get; set; } = "newest";

    /// <summary>ค้นหาชื่อเหตุการณ์หรือคำอธิบาย</summary>
    public string? Search { get; set; }

    /// <summary>กรองตาม EmployeeId (6 หลัก)</summary>
    public string? EmployeeId { get; set; }

    /// <summary>กรองตามชื่อ-นามสกุลผู้สร้าง</summary>
    public string? OwnerName { get; set; }

    /// <summary>กรองตามหมวดหมู่</summary>
    public Guid? CategoryId { get; set; }

    /// <summary>กรองตามวันที่เริ่มต้น (yyyy-MM-dd)</summary>
    public DateOnly? DateFrom { get; set; }

    /// <summary>กรองตามวันที่สิ้นสุด (yyyy-MM-dd)</summary>
    public DateOnly? DateTo { get; set; }
}
