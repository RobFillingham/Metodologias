using Backend.Models.DTOs.CocomoThree;

namespace Backend.Models.DTOs.CocomoTwoStageOne;

/// <summary>
/// DTO for Estimation (COCOMO 2 Stage 1)
/// </summary>
public class EstimationCocomo2Stage1Dto
{
    public int EstimationId { get; set; }
    public int ProjectId { get; set; }
    public string EstimationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Selected ratings
    public string SelectedAexp { get; set; } = "nominal";
    public string SelectedPexp { get; set; } = "nominal";
    public string SelectedPrec { get; set; } = "nominal";
    public string SelectedRely { get; set; } = "nominal";
    public string SelectedTmsp { get; set; } = "nominal";

    // Calculated results
    public decimal? TotalFp { get; set; }
    public decimal? Sloc { get; set; }
    public decimal? Ksloc { get; set; }
    public decimal? AexpMultiplier { get; set; }
    public decimal? PexpMultiplier { get; set; }
    public decimal? PrecMultiplier { get; set; }
    public decimal? RelyMultiplier { get; set; }
    public decimal? TmspMultiplier { get; set; }
    public decimal? Eaf { get; set; }
    public decimal? EffortPm { get; set; }
    public decimal? EffortHours { get; set; }

    // Actual results
    public decimal? ActualEffortPm { get; set; }
    public decimal? ActualEffortHours { get; set; }
    public decimal? ActualSloc { get; set; }

    // Related entities
    public LanguageDto? Language { get; set; }
    public ParameterSetCocomo2Stage1Dto? ParameterSet { get; set; }
    public List<ComponentCocomo2Stage1Dto> Components { get; set; } = new();
}

/// <summary>
/// DTO for creating a new Estimation
/// </summary>
public class CreateEstimationCocomo2Stage1Dto
{
    public int ProjectId { get; set; }
    public string EstimationName { get; set; } = string.Empty;
    public int ParamSetId { get; set; }
    public int LanguageId { get; set; }
    
    public string SelectedAexp { get; set; } = "nominal";
    public string SelectedPexp { get; set; } = "nominal";
    public string SelectedPrec { get; set; } = "nominal";
    public string SelectedRely { get; set; } = "nominal";
    public string SelectedTmsp { get; set; } = "nominal";
}

/// <summary>
/// DTO for updating ratings
/// </summary>
public class UpdateRatingsCocomo2Stage1Dto
{
    public string? SelectedAexp { get; set; }
    public string? SelectedPexp { get; set; }
    public string? SelectedPrec { get; set; }
    public string? SelectedRely { get; set; }
    public string? SelectedTmsp { get; set; }
}

/// <summary>
/// DTO for updating actual results
/// </summary>
public class UpdateActualResultsCocomo2Stage1Dto
{
    public decimal? ActualEffortPm { get; set; }
    public decimal? ActualEffortHours { get; set; }
    public decimal? ActualSloc { get; set; }
}
