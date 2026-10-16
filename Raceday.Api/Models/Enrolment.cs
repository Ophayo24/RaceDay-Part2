using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.Models;

public class Enrolment
{
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public int EventId { get; set; }
    public int CategoryId { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Confirmed"; // Pending | Confirmed | Cancelled

    public User? Participant { get; set; }
    public Event? Event { get; set; }
    public Category? Category { get; set; }
    public Result? Result { get; set; }
}
