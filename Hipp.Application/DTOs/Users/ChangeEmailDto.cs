using System.ComponentModel.DataAnnotations;

namespace Hipp.Application.DTOs.Users;

public class ChangeEmailDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string NewEmail { get; set; }
} 