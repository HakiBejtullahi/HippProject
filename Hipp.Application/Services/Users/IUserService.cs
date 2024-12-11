using Hipp.Application.DTOs.Users;

namespace Hipp.Application.Services.Users;

public interface IUserService
{
    Task<(bool Succeeded, string UserId, UserDto User, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto model);
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> GetByEmailAsync(string email);
    Task LogUserActivityAsync(string userId, string action, string details);
    Task<bool> UpdateLastLoginAsync(string userId);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<bool> ChangePasswordAsync(ChangePasswordDto model);
    Task<bool> ChangeEmailAsync(ChangeEmailDto model);
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto model);
    Task<bool> ResetPasswordAsync(ResetPasswordDto model);
    Task<bool> InitiatePasswordResetAsync(string email);
    Task<bool> CompletePasswordResetAsync(string email, string token, string newPassword);
    Task<bool> InitiateEmailChangeAsync(string userId, string newEmail);
    Task<bool> ConfirmEmailChangeAsync(string userId, string token);
    Task<bool> SoftDeleteUserAsync(string userId, string deletedBy);
    Task<bool> HardDeleteUserAsync(string userId);
    Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm, string role, bool? isDeleted, int pageNumber, int pageSize);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
} 