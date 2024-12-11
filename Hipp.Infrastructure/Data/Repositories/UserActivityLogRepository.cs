using Dapper;
using Hipp.Domain.Entities.Audit;
using Hipp.Domain.Interfaces.Audit;
using Hipp.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace Hipp.Infrastructure.Data.Repositories;

public class UserActivityLogRepository : Domain.Interfaces.Audit.IUserActivityLogRepository
{
    private readonly string _connectionString;

    public UserActivityLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task LogActivityAsync(string userId, string action, string description, string ipAddress, string? additionalInfo = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_UserId", userId);
        parameters.Add("p_Action", action);
        parameters.Add("p_Description", description);
        parameters.Add("p_IpAddress", ipAddress);
        parameters.Add("p_AdditionalInfo", additionalInfo);

        await connection.ExecuteAsync(
            "sp_LogUserActivity",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<(IEnumerable<UserActivityLog> Logs, int TotalCount)> GetUserActivityLogsAsync(
        string? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_UserId", userId);
        parameters.Add("p_StartDate", startDate);
        parameters.Add("p_EndDate", endDate);
        parameters.Add("p_PageNumber", pageNumber);
        parameters.Add("p_PageSize", pageSize);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetUserActivityLogs",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var logs = await multi.ReadAsync<UserActivityLog>();
        var totalCount = await multi.ReadSingleAsync<int>();

        return (logs, totalCount);
    }

    public async Task ClearOldLogsAsync(int daysToKeep)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_DaysToKeep", daysToKeep);

        await connection.ExecuteAsync(
            "sp_ClearOldActivityLogs",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
} 