using DayOneAPI.Models.DTOs.Feed;

namespace DayOneAPI.Interfaces;

public interface IFeedService
{
    Task<FeedResponse> GetFeedAsync(FeedQuery query);
}
