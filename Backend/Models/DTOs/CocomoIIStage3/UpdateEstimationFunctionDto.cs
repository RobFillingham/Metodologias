using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for updating an existing COCOMO II Stage 3 EstimationFunction
/// </summary>
public class UpdateEstimationFunctionDto
{
    [Required]
    public int FunctionId { get; set; }

    [Required(ErrorMessage = "Function name is required")]
    [StringLength(255, ErrorMessage = "Function name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Function type is required")]
    [RegularExpression("^(EI|EO|EQ|ILF|EIF)$", ErrorMessage = "Type must be one of: EI, EO, EQ, ILF, EIF")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "DET is required")]
    [Range(1, int.MaxValue, ErrorMessage = "DET must be a positive number")]
    public int Det { get; set; }

    [Required(ErrorMessage = "RET/FTR is required")]
    [Range(1, int.MaxValue, ErrorMessage = "RET/FTR must be a positive number")]
    public int RetFtr { get; set; }
}