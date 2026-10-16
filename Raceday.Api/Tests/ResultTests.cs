using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raceday.Api.Controllers;
using Raceday.Api.DTOs;
using Raceday.Api.Models;
using Xunit;

namespace Raceday.Api.Tests;

public class ResultTests
{
    private static ResultsController BuildWithUser(Raceday.Api.Data.RaceDayDbContext db, int userId, string role)
    {
        var controller = new ResultsController(db);
        var identity = new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            }, "TestAuth");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
        return controller;
    }

    [Fact]
    public async Task Capture_AsOrganiser_Succeeds()
    {
        var db = TestHelpers.NewDb(System.Guid.NewGuid().ToString());
        db.Events.Add(new Event
        {
            EventId = 1,
            OrganiserId = 1,
            Name = "E",
            Description = "d",
            EventDate = System.DateTime.Today.AddDays(10),
            Location = "L",
            DistanceKm = 5,
            EventType = "Run"
        });
        db.Categories.Add(new Category { CategoryId = 1, EventId = 1, CategoryName = "Open" });
        db.Enrolments.Add(new Enrolment
        {
            EnrolmentId = 1,
            ParticipantId = 42,
            EventId = 1,
            CategoryId = 1,
            Status = "Confirmed"
        });
        await db.SaveChangesAsync();

        var controller = BuildWithUser(db, 1, "Organiser");
        var result = await controller.Capture(1, new CreateResultDto
        {
            FinishTime = System.TimeSpan.FromMinutes(52),
            FinishingPosition = 1
        });

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, obj.StatusCode);
    }

    [Fact]
    public async Task Capture_WhenNotOwnedByOrganiser_Forbids()
    {
        var db = TestHelpers.NewDb(System.Guid.NewGuid().ToString());
        db.Events.Add(new Event
        {
            EventId = 1,
            OrganiserId = 999,
            Name = "E",
            Description = "d",
            EventDate = System.DateTime.Today.AddDays(10),
            Location = "L",
            DistanceKm = 5,
            EventType = "Run"
        });
        db.Categories.Add(new Category { CategoryId = 1, EventId = 1, CategoryName = "Open" });
        db.Enrolments.Add(new Enrolment
        {
            EnrolmentId = 1,
            ParticipantId = 42,
            EventId = 1,
            CategoryId = 1,
            Status = "Confirmed"
        });
        await db.SaveChangesAsync();

        var controller = BuildWithUser(db, 1, "Organiser");
        var result = await controller.Capture(1, new CreateResultDto
        {
            FinishTime = System.TimeSpan.FromMinutes(52),
            FinishingPosition = 1
        });

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Capture_DuplicateResult_ReturnsConflict()
    {
        var db = TestHelpers.NewDb(System.Guid.NewGuid().ToString());
        db.Events.Add(new Event
        {
            EventId = 1,
            OrganiserId = 1,
            Name = "E",
            Description = "d",
            EventDate = System.DateTime.Today.AddDays(10),
            Location = "L",
            DistanceKm = 5,
            EventType = "Run"
        });
        db.Categories.Add(new Category { CategoryId = 1, EventId = 1, CategoryName = "Open" });
        db.Enrolments.Add(new Enrolment
        {
            EnrolmentId = 1,
            ParticipantId = 42,
            EventId = 1,
            CategoryId = 1,
            Status = "Confirmed"
        });
        await db.SaveChangesAsync();

        var controller = BuildWithUser(db, 1, "Organiser");
        var dto = new CreateResultDto
        {
            FinishTime = System.TimeSpan.FromMinutes(52),
            FinishingPosition = 1
        };
        await controller.Capture(1, dto);
        var second = await controller.Capture(1, dto);

        Assert.IsType<ConflictObjectResult>(second);
    }
}

