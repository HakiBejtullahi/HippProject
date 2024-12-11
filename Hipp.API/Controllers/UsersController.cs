using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hipp.Application.Services.Users;
using Hipp.Application.DTOs.Users;
using System.Security.Claims;

namespace Hipp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto model)
    {
        try 
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = validationErrors });
            }

            var (succeeded, userId, user, errors) = await _userService.CreateUserAsync(model);
            if (!succeeded)
            {
                return BadRequest(new { Errors = errors });
            }

            await _userService.LogUserActivityAsync(userId, "UserCreated", $"User created with role {model.Role}");
            return CreatedAtAction(nameof(GetById), new { id = userId }, user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An unexpected error occurred while creating the user." });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        // Only allow users to change their own password, or Admin to change any password
        if (!User.IsInRole("Admin") && User.FindFirstValue(ClaimTypes.NameIdentifier) != model.UserId)
        {
            return Forbid();
        }

        var result = await _userService.ChangePasswordAsync(model);
        if (!result)
        {
            return BadRequest("Failed to change password");
        }

        await _userService.LogUserActivityAsync(model.UserId, "PasswordChange", "Password was changed");
        return Ok();
    }

    [HttpPost("initiate-password-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> InitiatePasswordReset([FromBody] string email)
    {
        var result = await _userService.InitiatePasswordResetAsync(email);
        // Always return OK to prevent email enumeration
        return Ok();
    }

    [HttpPost("complete-password-reset")]
    [AllowAnonymous]
    public async Task<IActionResult> CompletePasswordReset([FromBody] ResetPasswordDto model)
    {
        var result = await _userService.ResetPasswordAsync(model);
        if (!result)
        {
            return BadRequest("Invalid or expired token");
        }

        return Ok();
    }

    [HttpPut("{userId}/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(string userId, [FromBody] UpdateProfileDto model)
    {
        // Only allow users to update their own profile, or Admin to update any profile
        if (!User.IsInRole("Admin") && User.FindFirstValue(ClaimTypes.NameIdentifier) != userId)
        {
            return Forbid();
        }

        var result = await _userService.UpdateProfileAsync(userId, model);
        if (!result)
        {
            return BadRequest("Failed to update profile");
        }

        await _userService.LogUserActivityAsync(userId, "ProfileUpdate", "Profile information was updated");
        return Ok();
    }

    [HttpDelete("{userId}/soft")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDeleteUser(string userId)
    {
        var deletedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _userService.SoftDeleteUserAsync(userId, deletedBy);
        if (!result)
        {
            return BadRequest("Failed to delete user");
        }

        await _userService.LogUserActivityAsync(userId, "UserDeleted", "User was soft deleted");
        return Ok();
    }

    [HttpDelete("{userId}/hard")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HardDeleteUser(string userId)
    {
        var result = await _userService.HardDeleteUserAsync(userId);
        if (!result)
        {
            return BadRequest("Failed to delete user");
        }

        return Ok();
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers(
        [FromQuery] string searchTerm,
        [FromQuery] string role,
        [FromQuery] bool? isDeleted,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var users = await _userService.SearchUsersAsync(searchTerm, role, isDeleted, pageNumber, pageSize);
        return Ok(users);
    }

    [HttpGet("{userId}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        var roles = await _userService.GetUserRolesAsync(userId);
        return Ok(roles);
    }
} 