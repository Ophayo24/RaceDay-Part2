using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Api.Data;
using Raceday.Api.DTOs;
using Raceday.Api.Services;

namespace Raceday.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public UsersController(RaceDayDbContext db) => _db = db;

    /// <summary>Returns the currently logged-in user's profile.</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var id = User.GetUserId();
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new UserProfileDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            Phone = user.Phone,
            ProfilePictureUrl = user.ProfilePictureUrl
        });
    }

    /// <summary>Updates the currently logged-in user's profile.</summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var id = User.GetUserId();
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Phone = dto.Phone;
        user.ProfilePictureUrl = dto.ProfilePictureUrl;
        await _db.SaveChangesAsync();

        return Ok(new UserProfileDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            Phone = user.Phone,
            ProfilePictureUrl = user.ProfilePictureUrl
        });
    }

    /// <summary>Returns the logged-in Participant's personal race results.</summary>
    [HttpGet("me/results")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> MyResults()
    {
        var id = User.GetUserId();
        var results = await _db.Results
            .Include(r => r.Enrolment!).ThenInclude(e => e.Event)
            .Include(r => r.Enrolment!).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment!.ParticipantId == id)
            .Select(r => new
            {
                r.ResultId,
                r.EnrolmentId,
                EventName = r.Enrolment!.Event!.Name,
                EventDate = r.Enrolment.Event.EventDate,
                Category = r.Enrolment.Category!.CategoryName,
                r.FinishTime,
                r.FinishingPosition
            })
            .ToListAsync();

        return Ok(results);
    }
}