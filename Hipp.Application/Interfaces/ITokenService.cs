using Hipp.Domain.Entities.Identity;

namespace Hipp.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, string role);
    bool ValidateToken(string token);
    string GetUserIdFromToken(string token);
    string GetRoleFromToken(string token);
} 