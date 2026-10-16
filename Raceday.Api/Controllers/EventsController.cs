using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Api.Data;
using Raceday.Api.DTOs;
using Raceday.Api.Models;
using Raceday.Api.Services;

namespace Raceday.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    /// <summary>Returns all upcoming RaceDay events.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events
            .Where(e => e.EventDate >= DateTime.Today)
            .OrderBy(e => e.EventDate)
            .Select(e => ToDto(e))
            .ToListAsync();
        return Ok(events);
    }

    /// <summary>Returns details for one event.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Events.FindAsync(id);
        return e == null ? NotFound() : Ok(ToDto(e));
    }

    /// <summary>Creates a new event. Organisers only.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(CreateEventDto dto)
    {
        if (dto.EventType is not ("Run" or "Walk" or "Cycle"))
            return BadRequest(new { error = "EventType must be Run, Walk or Cycle." });

        var ev = new Event
        {
            OrganiserId = User.GetUserId(),
            Name = dto.Name,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Location = dto.Location,
            DistanceKm = dto.DistanceKm,
            EventType = dto.EventType,
            BannerImageUrl = dto.BannerImageUrl
        };
        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return StatusCode(201, ToDto(ev));
    }

    /// <summary>Updates an event created by the Organiser.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, CreateEventDto dto)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != User.GetUserId()) return Forbid();
        if (dto.EventType is not ("Run" or "Walk" or "Cycle"))
            return BadRequest(new { error = "EventType must be Run, Walk or Cycle." });

        ev.Name = dto.Name;
        ev.Description = dto.Description;
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location;
        ev.DistanceKm = dto.DistanceKm;
        ev.EventType = dto.EventType;
        ev.BannerImageUrl = dto.BannerImageUrl;
        await _db.SaveChangesAsync();
        return Ok(ToDto(ev));
    }

    /// <summary>Deletes an event managed by the Organiser.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != User.GetUserId()) return Forbid();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Returns route information for an event.</summary>
    [HttpGet("{id:int}/route")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRoute(int id)
    {
        var route = await _db.EventRoutes.FirstOrDefaultAsync(r => r.EventId == id);
        if (route == null) return NotFound();
        return Ok(new EventRouteDto
        {
            RouteId = route.RouteId,
            EventId = route.EventId,
            RouteName = route.RouteName,
            RouteDescription = route.RouteDescription,
            RouteUrl = route.RouteUrl
        });
    }

    private static EventDto ToDto(Event e) => new()
    {
        EventId = e.EventId,
        OrganiserId = e.OrganiserId,
        Name = e.Name,
        Description = e.Description,
        EventDate = e.EventDate,
        Location = e.Location,
        DistanceKm = e.DistanceKm,
        EventType = e.EventType,
        BannerImageUrl = e.BannerImageUrl
    };
}
