using Hipp.Application.DTOs.Auth;
using Hipp.Application.DTOs.Users;

namespace Hipp.Application.Interfaces.Identity;

public interface IUserService
{
    Task<(bool Succeeded, string UserId, UserDto User, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto model);
    Task<UserDto> GetByIdAsync(string id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<bool> UpdateAsync(UpdateUserDto model);
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto model);
    Task<bool> ChangePasswordAsync(ChangePasswordDto model);
    Task<bool> ChangeEmailAsync(ChangeEmailDto model);
    Task<bool> ResetPasswordAsync(ResetPasswordDto model);
    Task<bool> InitiatePasswordResetAsync(string email);
    Task<bool> CompletePasswordResetAsync(string email, string token, string newPassword);
    Task<bool> InitiateEmailChangeAsync(string userId, string newEmail);
    Task<bool> ConfirmEmailChangeAsync(string userId, string token);
    Task<bool> SoftDeleteUserAsync(string userId, string deletedBy);
    Task<bool> HardDeleteUserAsync(string userId);
    Task<IEnumerable<UserDto>> SearchUsersAsync(UserSearchDto searchParams);
    Task<bool> UpdateLastLoginAsync(string userId);
    Task LogUserActivityAsync(string userId, string action, string details);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
} 