using Hipp.Domain.Entities.Identity;

namespace Hipp.Domain.Entities.Audit;

public class UserActivityLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public DateTime Timestamp { get; set; }
} 