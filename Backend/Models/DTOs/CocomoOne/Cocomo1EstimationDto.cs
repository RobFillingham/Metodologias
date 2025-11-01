using System;

namespace Backend.Models.DTOs.CocomoOne;

public class Cocomo1EstimationDto
{
    public int Cocomo1EstimationId { get; set; }
    public int ProjectId { get; set; }
    public string EstimationName { get; set; } = string.Empty;
    public decimal Kloc { get; set; }
    public string Mode { get; set; } = string.Empty;
    public decimal? EffortPm { get; set; }
    public decimal? TdevMonths { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
