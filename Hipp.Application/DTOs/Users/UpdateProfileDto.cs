using System.ComponentModel.DataAnnotations;

namespace Hipp.Application.DTOs.Users;

public class UpdateProfileDto
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; }
} 