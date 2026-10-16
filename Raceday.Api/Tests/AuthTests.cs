using Microsoft.AspNetCore.Mvc;
using Raceday.Api.Controllers;
using Raceday.Api.DTOs;
using Raceday.Api.Services;
using Xunit;

namespace Raceday.Api.Tests;

public class AuthTests
{
    [Fact]
    public async Task Register_WithNewEmail_Succeeds()
    {
        var db = TestHelpers.NewDb(nameof(Register_WithNewEmail_Succeeds));
        var controller = new AuthController(db, new PasswordHasher());

        var result = await controller.Register(new RegisterDto
        {
            FirstName = "Sipho",
            LastName = "Nkosi",
            Email = "sipho@test.co.za",
            Password = "Pass123!",
            Role = "Participant"
        });

        Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var db = TestHelpers.NewDb(nameof(Register_WithDuplicateEmail_ReturnsConflict));
        var controller = new AuthController(db, new PasswordHasher());

        var dto = new RegisterDto
        {
            FirstName = "A",
            LastName = "B",
            Email = "dupe@test.co.za",
            Password = "Pass123!",
            Role = "Participant"
        };
        await controller.Register(dto);
        var second = await controller.Register(dto);

        Assert.IsType<ConflictObjectResult>(second);
    }

    [Fact]
    public async Task Register_WithInvalidRole_ReturnsBadRequest()
    {
        var db = TestHelpers.NewDb(nameof(Register_WithInvalidRole_ReturnsBadRequest));
        var controller = new AuthController(db, new PasswordHasher());

        var result = await controller.Register(new RegisterDto
        {
            FirstName = "A",
            LastName = "B",
            Email = "x@test.co.za",
            Password = "Pass123!",
            Role = "Admin"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_WithBadPassword_ReturnsUnauthorized()
    {
        var db = TestHelpers.NewDb(nameof(Login_WithBadPassword_ReturnsUnauthorized));
        var controller = new AuthController(db, new PasswordHasher());
        await controller.Register(new RegisterDto
        {
            FirstName = "A",
            LastName = "B",
            Email = "u@test.co.za",
            Password = "Correct1!",
            Role = "Participant"
        });

        var result = await controller.Login(new LoginDto
        {
            Email = "u@test.co.za",
            Password = "Wrong1!"
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
