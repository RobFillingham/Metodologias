using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs;

/// <summary>
/// DTO for updating a Function Point estimation
/// </summary>
public class UpdateFunctionPointEstimationDto
{
    [Required(ErrorMessage = "Estimation ID is required")]
    public int FpEstimationId { get; set; }

    [Required(ErrorMessage = "Project ID is required")]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Estimation name is required")]
    [StringLength(255, ErrorMessage = "Estimation name must not exceed 255 characters")]
    public string EstimationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "External inputs count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "External inputs must be >= 0")]
    public int ExternalInputs { get; set; }

    [Required(ErrorMessage = "External outputs count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "External outputs must be >= 0")]
    public int ExternalOutputs { get; set; }

    [Required(ErrorMessage = "External inquiries count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "External inquiries must be >= 0")]
    public int ExternalInquiries { get; set; }

    [Required(ErrorMessage = "Internal logical files count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Internal logical files must be >= 0")]
    public int InternalLogicalFiles { get; set; }

    [Required(ErrorMessage = "External interface files count is required")]
    [Range(0, int.MaxValue, ErrorMessage = "External interface files must be >= 0")]
    public int ExternalInterfaceFiles { get; set; }

    [Required(ErrorMessage = "Complexity level is required")]
    [StringLength(50, ErrorMessage = "Complexity level must not exceed 50 characters")]
    public string ComplexityLevel { get; set; } = "AVERAGE";

    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? Notes { get; set; }
}
