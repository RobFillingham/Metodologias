using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for updating actual results of an Estimation (post-mortem)
/// </summary>
public class UpdateActualResultsDto
{
    [Range(0, double.MaxValue, ErrorMessage = "Actual effort must be a positive number")]
    public decimal? ActualEffortPm { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Actual duration must be a positive number")]
    public decimal? ActualTdevMonths { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Actual SLOC must be a positive number")]
    public decimal? ActualSloc { get; set; }
}
