using System.ComponentModel.DataAnnotations;

namespace Hipp.Application.DTOs.Users;

public class ChangePasswordDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; }
} 