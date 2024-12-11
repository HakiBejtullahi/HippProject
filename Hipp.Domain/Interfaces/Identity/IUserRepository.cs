using Hipp.Domain.Entities.Identity;

namespace Hipp.Domain.Interfaces.Identity;

public interface IUserRepository
{
    Task<ApplicationUser> GetByIdAsync(string id);
    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<bool> CreateAsync(ApplicationUser user, string password);
    Task<bool> UpdateAsync(ApplicationUser user);
    Task<bool> DeleteAsync(string id);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<bool> UpdateLastLoginAsync(string userId);
} 