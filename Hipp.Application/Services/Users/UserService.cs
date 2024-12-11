using AutoMapper;
using Hipp.Application.DTOs.Users;
using Hipp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Hipp.Infrastructure.Data.Context;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Hipp.Domain.Entities.Audit;
namespace Hipp.Application.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly string _connectionString;

    public UserService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _mapper = mapper;
        
        var dbHost = configuration["DB_HOST"];
        var dbName = configuration["DB_NAME"];
        var dbUser = configuration["DB_USER"];
        var dbPass = configuration["DB_PASS"];
        
        _connectionString = $"Server={dbHost};Database={dbName};User={dbUser};Password={dbPass}";
    }

    public async Task<(bool Succeeded, string UserId, UserDto User, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto model)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return (false, null, null, result.Errors.Select(e => e.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return (false, null, null, roleResult.Errors.Select(e => e.Description));
            }

          //  using var connection = new MySqlConnection(_connectionString);
          //  var parameters = new DynamicParameters();
          //  parameters.Add("p_Id", user.Id);
          //  parameters.Add("p_Role", model.Role);
          //  await connection.ExecuteAsync(
           //     "CreateUserRole",
           //     parameters,
           //     commandType: CommandType.StoredProcedure
           // );

            var createdUser = await GetByIdAsync(user.Id);
            if (createdUser == null)
            {
                return (false, user.Id, null, new[] { "User created but could not be retrieved" });
            }

            return (true, user.Id, createdUser, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return (false, null, null, new[] { ex.Message });
        }
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var userDto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Role = roles.FirstOrDefault();

        return userDto;
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        var userDto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Role = roles.FirstOrDefault();

        return userDto;
    }

    public async Task LogUserActivityAsync(string userId, string action, string details)
    {
        var activity = new UserActivityLog
        {
            UserId = userId,
            Action = action,
            Description = details,
            IpAddress = "System",
            Timestamp = DateTime.UtcNow
        };

        _context.Set<UserActivityLog>().Add(activity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateLastLoginAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.LastLogin = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.FirstOrDefault();
            userDtos.Add(userDto);
        }

        return userDtos;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto model)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_UserId", model.UserId);
            parameters.Add("p_NewPasswordHash", model.NewPassword);
            
            await connection.ExecuteAsync(
                "ChangeUserPassword",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ChangeEmailAsync(ChangeEmailDto model)
    {
        Console.WriteLine("\n=== Starting ChangeEmailAsync ===");
        Console.WriteLine($"Input - UserId: {model.UserId}");
        
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_UserId", model.UserId);
            parameters.Add("p_NewEmail", model.NewEmail);
            parameters.Add("p_EmailVerificationToken", Guid.NewGuid().ToString());
            
            Console.WriteLine("Debug - Parameters created:");
            Console.WriteLine($"  p_UserId: {model.UserId}");
            Console.WriteLine($"  p_NewEmail: {model.NewEmail}");
            Console.WriteLine("  p_EmailVerificationToken: ****");
            
            Console.WriteLine("Debug - Executing stored procedure 'InitiateEmailChange'");
            await connection.ExecuteAsync(
                "InitiateEmailChange",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n=== Error in ChangeEmailAsync ===");
            Console.WriteLine($"Error Type: {ex.GetType().FullName}");
            Console.WriteLine($"Error Message: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return false;
        }
        finally
        {
            Console.WriteLine("=== Completed ChangeEmailAsync ===\n");
        }
    }

    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto model)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_UserId", userId);
            parameters.Add("p_FirstName", model.FirstName);
            parameters.Add("p_LastName", model.LastName);
            parameters.Add("p_PhoneNumber", model.PhoneNumber);

            await connection.ExecuteAsync(
                "UpdateUserProfile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_Email", model.Email);
            parameters.Add("p_Token", model.Token);
            parameters.Add("p_NewPasswordHash", model.NewPassword);
            
            await connection.ExecuteAsync(
                "CompletePasswordReset",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var user = await GetByEmailAsync(model.Email);
            if (user != null)
            {
                await LogUserActivityAsync(user.Id, "PasswordReset", "Password was reset using reset token");
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> InitiatePasswordResetAsync(string email)
    {
        using var connection = new MySqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Email", email);
        parameters.Add("@ResetToken", Guid.NewGuid().ToString());

        try
        {
            await connection.ExecuteAsync(
                "InitiatePasswordReset",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CompletePasswordResetAsync(string email, string token, string newPassword)
    {
        Console.WriteLine("\n=== Starting CompletePasswordResetAsync ===");
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_Email", email);
            parameters.Add("p_ResetToken", token);
            parameters.Add("p_NewPasswordHash", newPassword);
            
            Console.WriteLine($"Debug - Executing CompletePasswordReset for email: {email}");
            await connection.ExecuteAsync(
                "CompletePasswordReset",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CompletePasswordResetAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<bool> InitiateEmailChangeAsync(string userId, string newEmail)
    {
        using var connection = new MySqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@NewEmail", newEmail);
        parameters.Add("@EmailVerificationToken", Guid.NewGuid().ToString());

        try
        {
            await connection.ExecuteAsync(
                "InitiateEmailChange",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ConfirmEmailChangeAsync(string userId, string token)
    {
        using var connection = new MySqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Token", token);

        try
        {
            await connection.ExecuteAsync(
                "ConfirmEmailChange",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SoftDeleteUserAsync(string userId, string deletedBy)
    {
        Console.WriteLine("\n=== Starting SoftDeleteUserAsync ===");
        try
        {
            // Update the user's DeletedAt and DeletedBy fields
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = deletedBy;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SoftDeleteUserAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<bool> HardDeleteUserAsync(string userId)
    {
        Console.WriteLine("\n=== Starting HardDeleteUserAsync ===");
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_Id", userId);
            
            Console.WriteLine($"Debug - Executing HardDeleteUser for userId: {userId}");
            await connection.ExecuteAsync(
                "HardDeleteUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HardDeleteUserAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm, string role, bool? isDeleted, int pageNumber, int pageSize)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_SearchTerm", searchTerm);
            parameters.Add("p_Role", role);
            parameters.Add("p_IsDeleted", isDeleted.HasValue ? (isDeleted.Value ? 1 : 0) : (object)null);
            parameters.Add("p_PageNumber", pageNumber);
            parameters.Add("p_PageSize", pageSize);

            Console.WriteLine("Debug - Executing SearchUsers with parameters:");
            Console.WriteLine($"  p_SearchTerm: {searchTerm}");
            Console.WriteLine($"  p_Role: {role}");
            Console.WriteLine($"  p_IsDeleted: {(isDeleted.HasValue ? (isDeleted.Value ? 1 : 0) : "null")}");
            Console.WriteLine($"  p_PageNumber: {pageNumber}");
            Console.WriteLine($"  p_PageSize: {pageSize}");

            var users = await connection.QueryAsync<UserDto>(
                "SearchUsers",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return users;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SearchUsersAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        Console.WriteLine("\n=== Starting GetUserRolesAsync ===");
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("p_UserId", userId);
            
            Console.WriteLine($"Debug - Executing GetUserRoles for userId: {userId}");
            var roles = await connection.QueryAsync<string>(
                "GetUserRoles",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return roles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserRolesAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
} 