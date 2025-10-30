namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for EstimationFunction entity
/// </summary>
public class EstimationFunctionDto
{
    public int FunctionId { get; set; }
    public int EstimationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? Det { get; set; }
    public int? RetFtr { get; set; }
    public string? Complexity { get; set; }
    public decimal? CalculatedPoints { get; set; }
}
