namespace Backend.Models.DTOs;

/// <summary>
/// DTO for reading KLOC estimation data
/// </summary>
public class KlocEstimationDto
{
    public int KlocEstimationId { get; set; }

    public int ProjectId { get; set; }

    public string EstimationName { get; set; } = string.Empty;

    public int LinesOfCode { get; set; }

    public decimal? EstimatedEffort { get; set; }

    public decimal? EstimatedCost { get; set; }

    public decimal? EstimatedTime { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
