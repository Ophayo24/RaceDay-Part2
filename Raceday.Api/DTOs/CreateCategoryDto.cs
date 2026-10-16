using System.ComponentModel.DataAnnotations;

namespace Raceday.Api.DTOs;

public class CreateCategoryDto
{
    [Required, MaxLength(100)] public string CategoryName { get; set; } = "";
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? CategoryDistanceKm { get; set; }
}

