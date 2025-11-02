using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs;

/// <summary>
/// DTO for updating a Use Case Point estimation
/// </summary>
public class UpdateUseCasePointEstimationDto
{
    [Required(ErrorMessage = "Estimation ID is required")]
    public int UcpEstimationId { get; set; }

    [Required(ErrorMessage = "Project ID is required")]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Estimation name is required")]
    [StringLength(255, ErrorMessage = "Estimation name must not exceed 255 characters")]
    public string EstimationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Simple use cases count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Simple use cases must be >= 0")]
    public int SimpleUccCount { get; set; }

    [Required(ErrorMessage = "Average use cases count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Average use cases must be >= 0")]
    public int AverageUccCount { get; set; }

    [Required(ErrorMessage = "Complex use cases count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Complex use cases must be >= 0")]
    public int ComplexUccCount { get; set; }

    [Required(ErrorMessage = "Simple actors count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Simple actors must be >= 0")]
    public int SimpleActorCount { get; set; }

    [Required(ErrorMessage = "Average actors count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Average actors must be >= 0")]
    public int AverageActorCount { get; set; }

    [Required(ErrorMessage = "Complex actors count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Complex actors must be >= 0")]
    public int ComplexActorCount { get; set; }

    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? Notes { get; set; }
}
