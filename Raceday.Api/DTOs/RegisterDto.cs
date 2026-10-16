using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.DTOs;

public class RegisterDto
{
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = "";
    [Required, MinLength(6)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "Participant";
    [MaxLength(20)] public string? Phone { get; set; }
}

