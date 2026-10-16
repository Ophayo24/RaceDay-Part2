using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.Models;

public class Result
{
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }

    public Enrolment? Enrolment { get; set; }
}
