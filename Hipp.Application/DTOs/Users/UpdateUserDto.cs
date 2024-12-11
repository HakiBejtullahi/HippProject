using System.ComponentModel.DataAnnotations;

namespace Hipp.Application.DTOs.Users;

public class UpdateUserDto
{
    [Required]
    public string Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; }
} 