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
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;

    /// <summary>Lists categories available for an event.</summary>
    [HttpGet("events/{eventId:int}/categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        var cats = await _db.Categories
                       .Where(c => c.EventId == eventId)
                       .Select(c => ToDto(c))
                       .ToListAsync();
        return Ok(cats);
    }

    /// <summary>Creates a new category for an event. Organisers only.</summary>
    [HttpPost("events/{eventId:int}/categories")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, CreateCategoryDto dto)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != User.GetUserId()) return Forbid();

        var cat = new Category
        {
            EventId = eventId,
            CategoryName = dto.CategoryName,
            MinAge = dto.MinAge,
            MaxAge = dto.MaxAge,
            CategoryDistanceKm = dto.CategoryDistanceKm
        };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return StatusCode(201, ToDto(cat));
    }

    /// <summary>Updates an existing event category. Organisers only.</summary>
    [HttpPut("categories/{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, CreateCategoryDto dto)
    {
        var cat = await _db.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);
        if (cat == null) return NotFound();
        if (cat.Event!.OrganiserId != User.GetUserId()) return Forbid();

        cat.CategoryName = dto.CategoryName;
        cat.MinAge = dto.MinAge;
        cat.MaxAge = dto.MaxAge;
        cat.CategoryDistanceKm = dto.CategoryDistanceKm;
        await _db.SaveChangesAsync();
        return Ok(ToDto(cat));
    }

    /// <summary>Deletes an event category. Organisers only.</summary>
    [HttpDelete("categories/{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);
        if (cat == null) return NotFound();
        if (cat.Event!.OrganiserId != User.GetUserId()) return Forbid();

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static CategoryDto ToDto(Category c) => new()
    {
        CategoryId = c.CategoryId,
        EventId = c.EventId,
        CategoryName = c.CategoryName,
        MinAge = c.MinAge,
        MaxAge = c.MaxAge,
        CategoryDistanceKm = c.CategoryDistanceKm
    };
}
