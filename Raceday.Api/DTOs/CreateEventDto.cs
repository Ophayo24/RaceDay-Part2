using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.DTOs;

public class CreateEventDto
{
    [Required, MaxLength(150)] public string Name { get; set; } = "";
    [Required, MaxLength(1000)] public string Description { get; set; } = "";
    [Required] public DateTime EventDate { get; set; }
    [Required, MaxLength(200)] public string Location { get; set; } = "";
    [Range(0.01, 10000)] public decimal DistanceKm { get; set; }
    [Required] public string EventType { get; set; } = ""; // Run | Walk | Cycle
    [MaxLength(500)] public string? BannerImageUrl { get; set; }
}
