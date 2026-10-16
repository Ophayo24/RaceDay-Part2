using System.Security.Claims;
using Raceday.Api.Models;

namespace Raceday.Api.Services;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(id ?? "0");
    }

    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? "";

    public static List<Claim> ToClaims(this User u) => new()
        {
            new Claim(ClaimTypes.NameIdentifier, u.UserId.ToString()),
            new Claim(ClaimTypes.Email, u.Email),
            new Claim(ClaimTypes.Name, $"{u.FirstName} {u.LastName}"),
            new Claim(ClaimTypes.Role, u.Role)
        };
}
