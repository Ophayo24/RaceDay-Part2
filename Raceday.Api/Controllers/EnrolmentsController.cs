using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Api.Data;
using Raceday.Api.DTOs;
using Raceday.Api.Models;
using Raceday.Api.Services;

namespace Raceday.Api.Controllers;

[ApiController]
[Route("api/enrolments")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    /// <summary>Creates a new enrolment for the logged-in Participant.</summary>
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Create(CreateEnrolmentDto dto)
    {
        var participantId = User.GetUserId();

        var ev = await _db.Events.FindAsync(dto.EventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        var cat = await _db.Categories.FindAsync(dto.CategoryId);
        if (cat == null || cat.EventId != dto.EventId)
            return NotFound(new { error = "Category not found for this event." });

        var exists = await _db.Enrolments.AnyAsync(e =>
            e.ParticipantId == participantId && e.EventId == dto.EventId);
        if (exists) return Conflict(new { error = "Already enrolled in this event." });

        var enrolment = new Enrolment
        {
            ParticipantId = participantId,
            EventId = dto.EventId,
            CategoryId = dto.CategoryId,
            Status = "Confirmed"
        };
        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return StatusCode(201, ToDto(enrolment));
    }

    /// <summary>Lists the logged-in Participant's own enrolments.</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Mine()
    {
        var id = User.GetUserId();
        var list = await _db.Enrolments
            .Where(e => e.ParticipantId == id)
            .Select(e => ToDto(e))
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>Returns all enrolments for a specific event. Organisers only.</summary>
    [HttpGet("event/{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != User.GetUserId()) return Forbid();

        var list = await _db.Enrolments
            .Include(e => e.Participant)
            .Include(e => e.Category)
            .Where(e => e.EventId == eventId)
            .Select(e => new
            {
                e.EnrolmentId,
                e.ParticipantId,
                ParticipantName = e.Participant!.FirstName + " " + e.Participant.LastName,
                e.CategoryId,
                CategoryName = e.Category!.CategoryName,
                e.EnrolmentDate,
                e.Status
            })
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>Cancels an existing enrolment. Participant only, own enrolment.</summary>
    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Cancel(int id)
    {
        var e = await _db.Enrolments.FindAsync(id);
        if (e == null) return NotFound();
        if (e.ParticipantId != User.GetUserId()) return Forbid();

        e.Status = "Cancelled";
        await _db.SaveChangesAsync();
        return Ok(ToDto(e));
    }

    private static EnrolmentDto ToDto(Enrolment e) => new()
    {
        EnrolmentId = e.EnrolmentId,
        ParticipantId = e.ParticipantId,
        EventId = e.EventId,
        CategoryId = e.CategoryId,
        EnrolmentDate = e.EnrolmentDate,
        Status = e.Status
    };
}
