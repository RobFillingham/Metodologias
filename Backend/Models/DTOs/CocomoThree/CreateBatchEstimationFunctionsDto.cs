using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for creating multiple EstimationFunctions in batch
/// </summary>
public class CreateBatchEstimationFunctionsDto
{
    [Required(ErrorMessage = "Functions list is required")]
    [MinLength(1, ErrorMessage = "At least one function is required")]
    public List<CreateEstimationFunctionDto> Functions { get; set; } = new();
}
