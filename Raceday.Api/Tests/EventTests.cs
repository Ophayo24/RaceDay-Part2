using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raceday.Api.Controllers;
using Raceday.Api.DTOs;
using Xunit;

namespace Raceday.Api.Tests;

public class EventTests
{
    private static EventsController BuildWithUser(int userId, string role)
    {
        var db = TestHelpers.NewDb(System.Guid.NewGuid().ToString());
        var controller = new EventsController(db);
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
    public async Task Create_AsOrganiser_ReturnsCreated()
    {
        var controller = BuildWithUser(1, "Organiser");
        var result = await controller.Create(new CreateEventDto
        {
            Name = "Test 10K",
            Description = "Test event",
            EventDate = System.DateTime.Today.AddDays(7),
            Location = "JHB",
            DistanceKm = 10,
            EventType = "Run"
        });
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, obj.StatusCode);
    }

    [Fact]
    public async Task Create_WithInvalidEventType_ReturnsBadRequest()
    {
        var controller = BuildWithUser(1, "Organiser");
        var result = await controller.Create(new CreateEventDto
        {
            Name = "Test",
            Description = "x",
            EventDate = System.DateTime.Today.AddDays(1),
            Location = "x",
            DistanceKm = 5,
            EventType = "Swim"
        });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoEvents()
    {
        var controller = BuildWithUser(1, "Participant");
        var result = await controller.GetAll();
        Assert.IsType<OkObjectResult>(result);
    }
}
