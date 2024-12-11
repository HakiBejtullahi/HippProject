using Hipp.Domain.Entities.Audit;

namespace Hipp.Domain.Interfaces;

public interface IUserActivityLogRepository
{
    Task<int> AddAsync(UserActivityLog log);
    Task<(IEnumerable<UserActivityLog> Logs, int TotalCount)> GetUserActivityLogsAsync(
        string? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<IEnumerable<UserActivityLog>> GetRecentActivityAsync(int count);
    Task<IEnumerable<UserActivityLog>> GetUserRecentActivityAsync(string userId, int count);
    Task<int> ClearOldLogsAsync(int daysToKeep);
    Task<UserActivityLog?> GetByIdAsync(int id);
} 