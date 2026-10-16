using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raceday.Api.Controllers;
using Raceday.Api.DTOs;
using Raceday.Api.Models;
using Xunit;

namespace Raceday.Api.Tests;

public class EnrolmentTests
{
    private static EnrolmentsController BuildWithUser(Raceday.Api.Data.RaceDayDbContext db, int userId, string role)
    {
        var controller = new EnrolmentsController(db);
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
    public async Task Create_AsParticipant_Succeeds()
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
        db.Categories.Add(new Category
        {
            CategoryId = 1,
            EventId = 1,
            CategoryName = "Open"
        });
        await db.SaveChangesAsync();

        var controller = BuildWithUser(db, 42, "Participant");
        var result = await controller.Create(new CreateEnrolmentDto
        {
            EventId = 1,
            CategoryId = 1
        });

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, obj.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateEnrolment_ReturnsConflict()
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
        db.Categories.Add(new Category
        {
            CategoryId = 1,
            EventId = 1,
            CategoryName = "Open"
        });
        await db.SaveChangesAsync();

        var controller = BuildWithUser(db, 42, "Participant");
        var dto = new CreateEnrolmentDto { EventId = 1, CategoryId = 1 };
        await controller.Create(dto);
        var second = await controller.Create(dto);

        Assert.IsType<ConflictObjectResult>(second);
    }

    [Fact]
    public async Task Create_WhenEventMissing_ReturnsNotFound()
    {
        var db = TestHelpers.NewDb(System.Guid.NewGuid().ToString());
        var controller = BuildWithUser(db, 42, "Participant");
        var result = await controller.Create(new CreateEnrolmentDto
        {
            EventId = 99,
            CategoryId = 99
        });
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
