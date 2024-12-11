using Hipp.Application.DTOs.Audit;

namespace Hipp.Application.Interfaces.Audit;

public interface IUserActivityLogService
{
    Task LogActivityAsync(string userId, string action, string description, string ipAddress, string? additionalInfo = null);
    Task<(IEnumerable<UserActivityLogDto> Logs, int TotalCount)> GetUserActivityLogsAsync(
        string? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task<IEnumerable<UserActivityLogDto>> GetRecentActivityAsync(int count);
    Task<IEnumerable<UserActivityLogDto>> GetUserRecentActivityAsync(string userId, int count);
    Task ClearOldLogsAsync(int daysToKeep);
    Task<UserActivityLogDto> GetByIdAsync(int id);
} 