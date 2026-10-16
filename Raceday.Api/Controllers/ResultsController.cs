using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Api.Data;
using Raceday.Api.DTOs;
using Raceday.Api.Models;
using Raceday.Api.Services;

namespace Raceday.Api.Controllers;

[ApiController]
[Route("api")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    /// <summary>Captures a Participant's finish time and position. Organisers only.</summary>
    [HttpPost("enrolments/{enrolmentId:int}/result")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Capture(int enrolmentId, CreateResultDto dto)
    {
        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId);
        if (enrolment == null) return NotFound();
        if (enrolment.Event!.OrganiserId != User.GetUserId()) return Forbid();

        var existing = await _db.Results.FirstOrDefaultAsync(r => r.EnrolmentId == enrolmentId);
        if (existing != null) return Conflict(new { error = "Result already captured." });

        var result = new Result
        {
            EnrolmentId = enrolmentId,
            FinishTime = dto.FinishTime,
            FinishingPosition = dto.FinishingPosition
        };
        _db.Results.Add(result);
        await _db.SaveChangesAsync();

        return StatusCode(201, new ResultDto
        {
            ResultId = result.ResultId,
            EnrolmentId = result.EnrolmentId,
            FinishTime = result.FinishTime,
            FinishingPosition = result.FinishingPosition
        });
    }

    /// <summary>Returns all results for an event. Organisers only.</summary>
    [HttpGet("events/{eventId:int}/results")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != User.GetUserId()) return Forbid();

        var results = await _db.Results
            .Include(r => r.Enrolment)
            .Where(r => r.Enrolment!.EventId == eventId)
            .Select(r => new ResultDto
            {
                ResultId = r.ResultId,
                EnrolmentId = r.EnrolmentId,
                FinishTime = r.FinishTime,
                FinishingPosition = r.FinishingPosition
            })
            .ToListAsync();
        return Ok(results);
    }
}
