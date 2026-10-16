using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.DTOs;

public class UpdateProfileDto
{
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(500)] public string? ProfilePictureUrl { get; set; }
}

