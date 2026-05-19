using DayOneAPI.Models.DTOs.Events;

namespace DayOneAPI.Models.DTOs.Dashboard;

public class DashboardResponse
{
    public int TodayCount { get; set; }
    public int ThisWeekCount { get; set; }
    public int ThisMonthCount { get; set; }
    public List<EventResponse> UpcomingEvents { get; set; } = [];
    public List<CategoryCount> EventsByCategory { get; set; } = [];
    public List<EventResponse> RecentlyAdded { get; set; } = [];
}

public class CategoryCount
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}
