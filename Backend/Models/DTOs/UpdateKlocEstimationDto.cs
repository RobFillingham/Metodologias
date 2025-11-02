using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs;

/// <summary>
/// DTO for updating a KLOC estimation
/// </summary>
public class UpdateKlocEstimationDto
{
    [Required(ErrorMessage = "Estimation ID is required")]
    public int KlocEstimationId { get; set; }

    [Required(ErrorMessage = "Project ID is required")]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Estimation name is required")]
    [StringLength(255, ErrorMessage = "Estimation name must not exceed 255 characters")]
    public string EstimationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lines of code is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Lines of code must be greater than 0")]
    public int LinesOfCode { get; set; }

    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? Notes { get; set; }
}
