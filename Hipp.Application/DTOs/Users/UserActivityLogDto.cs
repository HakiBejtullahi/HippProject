namespace Hipp.Application.DTOs.Users;

public class UserActivityLogDto
{
    public string UserId { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AdditionalInfo { get; set; }
} 