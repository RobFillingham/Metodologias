namespace Backend.Models.DTOs;

/// <summary>
/// DTO for reading Use Case Point estimation data
/// </summary>
public class UseCasePointEstimationDto
{
    public int UcpEstimationId { get; set; }

    public int ProjectId { get; set; }

    public string EstimationName { get; set; } = string.Empty;

    public int SimpleUccCount { get; set; }

    public int AverageUccCount { get; set; }

    public int ComplexUccCount { get; set; }

    public int SimpleActorCount { get; set; }

    public int AverageActorCount { get; set; }

    public int ComplexActorCount { get; set; }

    public decimal? UnadjustedUcp { get; set; }

    public decimal? TechnicalComplexityFactor { get; set; }

    public decimal? EnvironmentFactor { get; set; }

    public decimal? AdjustedUcp { get; set; }

    public decimal? EstimatedEffort { get; set; }

    public decimal? EstimatedEffortPm { get; set; }

    public decimal? EstimatedCost { get; set; }

    public decimal? EstimatedTime { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<UseCaseTechnicalFactorDto>? TechnicalFactors { get; set; }

    public ICollection<UseCaseEnvironmentFactorDto>? EnvironmentFactors { get; set; }
}

/// <summary>
/// DTO for Use Case technical factor
/// </summary>
public class UseCaseTechnicalFactorDto
{
    public int UcpTechFactorId { get; set; }

    public int UcpEstimationId { get; set; }

    public string FactorName { get; set; } = string.Empty;

    public int FactorWeight { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for Use Case environment factor
/// </summary>
public class UseCaseEnvironmentFactorDto
{
    public int UcpEnvFactorId { get; set; }

    public int UcpEstimationId { get; set; }

    public string FactorName { get; set; } = string.Empty;

    public int FactorWeight { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
