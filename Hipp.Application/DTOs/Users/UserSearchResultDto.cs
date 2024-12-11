namespace Hipp.Application.DTOs.Users;

public class UserSearchResultDto
{
    public IEnumerable<UserDto> Users { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
} 