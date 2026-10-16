using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.Models;

public class Event
{
    public int EventId { get; set; }
    public int OrganiserId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    [Required, MaxLength(20)]
    public string EventType { get; set; } = string.Empty; // Run | Walk | Cycle

    [MaxLength(500)]
    public string? BannerImageUrl { get; set; }

    public User? Organiser { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    public EventRoute? Route { get; set; }
}
