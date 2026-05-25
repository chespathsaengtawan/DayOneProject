using DayOneAPI.Interfaces;
using DayOneAPI.Models.DTOs.EventCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DayOneAPI.Controllers;

[ApiController]
[Route("event-categories")]
[Authorize]
public class EventCategoriesController : ControllerBase
{
    private readonly IEventCategoryService _service;

    public EventCategoriesController(IEventCategoryService service)
    {
        _service = service;
    }

    /// <summary>ดึงรายการหมวดหมู่ทั้งหมด</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
    {
        var categories = await _service.GetAllAsync(activeOnly);
        return Ok(categories);
    }

    /// <summary>ดึงหมวดหมู่ตาม ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category is null) return NotFound(new { message = "ไม่พบหมวดหมู่" });
        return Ok(category);
    }

    /// <summary>เพิ่มหมวดหมู่ใหม่</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventCategoryRequest request)
    {
        try
        {
            var category = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>แก้ไขหมวดหมู่</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventCategoryRequest request)
    {
        try
        {
            var category = await _service.UpdateAsync(id, request);
            if (category is null) return NotFound(new { message = "ไม่พบหมวดหมู่" });
            return Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>ลบหมวดหมู่ (ลบไม่ได้ถ้ามี event ใช้งานอยู่)</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "ไม่พบหมวดหมู่" });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>เปิด/ปิดการใช้งานหมวดหมู่</summary>
    [HttpPatch("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var category = await _service.ToggleActiveAsync(id);
        if (category is null) return NotFound(new { message = "ไม่พบหมวดหมู่" });
        return Ok(category);
    }
}
