using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.DTOs;

public class CreateEnrolmentDto
{
    [Required] public int EventId { get; set; }
    [Required] public int CategoryId { get; set; }
}

