namespace Raceday.Api.DTOs;

public class ResultDto
{
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public TimeSpan FinishTime { get; set; }
    public int FinishingPosition { get; set; }
}

