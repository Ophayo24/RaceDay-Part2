using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.DTOs;

public class CreateResultDto
{
    [Required] public TimeSpan FinishTime { get; set; }
    [Range(1, int.MaxValue)] public int FinishingPosition { get; set; }
}
