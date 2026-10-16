namespace Raceday.Api.DTOs;

public class EventRouteDto
{
    public int RouteId { get; set; }
    public int EventId { get; set; }
    public string RouteName { get; set; } = "";
    public string RouteDescription { get; set; } = "";
    public string? RouteUrl { get; set; }
}
