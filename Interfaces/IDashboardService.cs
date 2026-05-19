using DayOneAPI.Models.DTOs.Dashboard;

namespace DayOneAPI.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid userId);
}
