namespace Backend.Models.DTOs;

/// <summary>
/// DTO for reading Function Point estimation data
/// </summary>
public class FunctionPointEstimationDto
{
    public int FpEstimationId { get; set; }

    public int ProjectId { get; set; }

    public string EstimationName { get; set; } = string.Empty;

    public int ExternalInputs { get; set; }

    public int ExternalOutputs { get; set; }

    public int ExternalInquiries { get; set; }

    public int InternalLogicalFiles { get; set; }

    public int ExternalInterfaceFiles { get; set; }

    public string ComplexityLevel { get; set; } = string.Empty;

    public decimal? UnadjustedFp { get; set; }

    public decimal? ValueAdjustmentFactor { get; set; }

    public decimal? AdjustedFp { get; set; }

    public decimal? EstimatedEffort { get; set; }

    public decimal? EstimatedCost { get; set; }

    public decimal? EstimatedTime { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<FunctionPointCharacteristicDto>? Characteristics { get; set; }
}

/// <summary>
/// DTO for Function Point characteristic
/// </summary>
public class FunctionPointCharacteristicDto
{
    public int FpCharId { get; set; }

    public int FpEstimationId { get; set; }

    public string CharacteristicName { get; set; } = string.Empty;

    public string InfluenceLevel { get; set; } = string.Empty;

    public int Score { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
