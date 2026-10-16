using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.Models;

public class EventRoute
{
    public int RouteId { get; set; }
    public int EventId { get; set; }

    [Required, MaxLength(150)]
    public string RouteName { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string RouteDescription { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? RouteUrl { get; set; }

    public Event? Event { get; set; }
}
