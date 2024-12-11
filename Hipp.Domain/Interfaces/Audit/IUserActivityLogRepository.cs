using Hipp.Domain.Entities.Audit;

namespace Hipp.Domain.Interfaces.Audit;

public interface IUserActivityLogRepository
{
    Task LogActivityAsync(string userId, string action, string description, string ipAddress, string? additionalInfo = null);
    Task<(IEnumerable<UserActivityLog> Logs, int TotalCount)> GetUserActivityLogsAsync(
        string? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10);
    Task ClearOldLogsAsync(int daysToKeep);
} 