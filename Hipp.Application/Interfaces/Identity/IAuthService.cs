using Hipp.Application.DTOs.Auth;
using Hipp.Application.DTOs.Users;

namespace Hipp.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<(string token, UserDto user)> LoginAsync(LoginDto loginDto);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto> GetUserByTokenAsync(string token);
    Task<bool> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string userId);
} 