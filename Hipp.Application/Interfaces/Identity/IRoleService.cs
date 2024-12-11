using Hipp.Application.DTOs.Auth;
using Hipp.Application.DTOs.Users;
using Hipp.Application.DTOs.Roles;

namespace Hipp.Application.Interfaces.Identity;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleName);
    Task<bool> AssignUserToRoleAsync(string userId, string roleName);
    Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<bool> CreateRoleAsync(string roleName);
    Task<bool> DeleteRoleAsync(string roleName);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    
    // Role-specific user queries
    Task<IEnumerable<UserDto>> GetMenaxhersAsync();
    Task<IEnumerable<UserDto>> GetKomercialistsAsync();
    Task<IEnumerable<UserDto>> GetEtiketuesAsync();
    Task<IEnumerable<UserDto>> GetShofersAsync();
} 