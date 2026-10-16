namespace Raceday.Api.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public int EventId { get; set; }
    public string CategoryName { get; set; } = "";
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? CategoryDistanceKm { get; set; }
}
