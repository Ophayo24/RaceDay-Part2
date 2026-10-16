
using Microsoft.EntityFrameworkCore;
using Raceday.Api.Data;

namespace Raceday.Api.Tests;

public static class TestHelpers
{
    public static RaceDayDbContext NewDb(string name) =>
        new(new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(name)
            .Options);
}
