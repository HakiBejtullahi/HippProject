using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hipp.Domain.Entities.Identity;
using Hipp.Infrastructure.Data.Context;

namespace Hipp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public RolesController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet("menaxhers")]
    public async Task<IActionResult> GetMenaxhers()
    {
        var menaxhers = await _context.Menaxhers
            .Include(m => m.User)
            .Select(m => new { m.Id, m.User.FirstName, m.User.LastName, m.User.Email })
            .ToListAsync();
        return Ok(menaxhers);
    }

    [HttpGet("komercialists")]
    public async Task<IActionResult> GetKomercialists()
    {
        var komercialists = await _context.Komercialists
            .Include(k => k.User)
            .Select(k => new { k.Id, k.User.FirstName, k.User.LastName, k.User.Email })
            .ToListAsync();
        return Ok(komercialists);
    }

    [HttpGet("etiketueses")]
    public async Task<IActionResult> GetEtiketueses()
    {
        var etiketueses = await _context.Etiketueses
            .Include(e => e.User)
            .Select(e => new { e.Id, e.User.FirstName, e.User.LastName, e.User.Email, e.CompletedTasksCount })
            .ToListAsync();
        return Ok(etiketueses);
    }

    [HttpGet("shofers")]
    public async Task<IActionResult> GetShofers()
    {
        var shofers = await _context.Shofers
            .Include(s => s.User)
            .Select(s => new { s.Id, s.User.FirstName, s.User.LastName, s.User.Email })
            .ToListAsync();
        return Ok(shofers);
    }

    [HttpGet("user/{userId}/role")]
    public async Task<IActionResult> GetUserRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { Role = roles.FirstOrDefault() });
    }

    [HttpPost("user/{userId}/role")]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync(role))
            return BadRequest("Role does not exist");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Create role-specific entity
        switch (role.ToLower())
        {
            case "menaxher":
                if (!await _context.Menaxhers.AnyAsync(m => m.UserId == userId))
                    _context.Menaxhers.Add(new Menaxher { UserId = userId });
                break;
            case "komercialist":
                if (!await _context.Komercialists.AnyAsync(k => k.UserId == userId))
                    _context.Komercialists.Add(new Komercialist { UserId = userId });
                break;
            case "etiketues":
                if (!await _context.Etiketueses.AnyAsync(e => e.UserId == userId))
                    _context.Etiketueses.Add(new Etiketues { UserId = userId });
                break;
            case "shofer":
                if (!await _context.Shofers.AnyAsync(s => s.UserId == userId))
                    _context.Shofers.Add(new Shofer { UserId = userId });
                break;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
} 