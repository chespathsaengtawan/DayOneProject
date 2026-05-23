using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.Feed;
using Microsoft.AspNetCore.Mvc;

namespace DayOneAPI.Controllers;

[ApiController]
[Route("api/feed")]
public class FeedController : ControllerBase
{
    private readonly IFeedService _feedService;

    public FeedController(IFeedService feedService)
    {
        _feedService = feedService;
    }

    /// <summary>
    /// ดึง feed เหตุการณ์ทั้งหมดในระบบ (ชุดละ 10 รายการ)
    /// </summary>
    /// <param name="query">
    /// sort: newest | oldest | event_asc | event_desc
    /// search: ค้นหาในชื่อ/คำอธิบาย
    /// employeeId: รหัสพนักงาน 6 หลัก
    /// ownerName: ชื่อ-นามสกุลผู้สร้าง
    /// categoryId: UUID ของหมวดหมู่
    /// dateFrom / dateTo: วันที่ (yyyy-MM-dd)
    /// page: หน้าที่ต้องการ (เริ่มที่ 1)
    /// pageSize: จำนวนต่อหน้า (1-50, ค่าเริ่มต้น 10)
    /// </param>
    [HttpGet]
    public async Task<IActionResult> GetFeed([FromQuery] FeedQuery query)
    {
        var feed = await _feedService.GetFeedAsync(query);
        return Ok(feed);
    }
}
