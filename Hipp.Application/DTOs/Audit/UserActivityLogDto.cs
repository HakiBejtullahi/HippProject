namespace Hipp.Application.DTOs.Audit;

public class UserActivityLogDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public DateTime Timestamp { get; set; }
} 