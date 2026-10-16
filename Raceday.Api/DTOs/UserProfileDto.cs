namespace Raceday.Api.DTOs;

public class UserProfileDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Phone { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
