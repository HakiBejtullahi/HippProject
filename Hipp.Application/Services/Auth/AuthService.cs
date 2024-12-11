using AutoMapper;
using Hipp.Application.DTOs.Auth;
using Hipp.Application.DTOs.Users;
using Hipp.Application.Interfaces;
using Hipp.Application.Services.Users;
using Hipp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hipp.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IUserService userService,
        IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<(string token, UserDto user)> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();
        if (string.IsNullOrEmpty(role))
        {
            throw new UnauthorizedAccessException("User has no assigned role");
        }

        await _userService.UpdateLastLoginAsync(user.Id);
        await _userService.LogUserActivityAsync(user.Id, "Login", "User logged in successfully");

        var token = _tokenService.GenerateToken(user, role);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Role = role;

        return (token, userDto);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _tokenService.ValidateToken(token);
    }

    public async Task<UserDto> GetUserByTokenAsync(string token)
    {
        if (!_tokenService.ValidateToken(token))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        var userId = _tokenService.GetUserIdFromToken(token);
        return await _userService.GetByIdAsync(userId);
    }
} 