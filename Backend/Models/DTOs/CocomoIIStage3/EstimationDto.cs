namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for COCOMO II Stage 3 Estimation entity with calculated results
/// </summary>
public class EstimationDto
{
    public int EstimationId { get; set; }
    public int ProjectId { get; set; }
    public string EstimationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Related entities
    public LanguageDto? Language { get; set; }
    public ParameterSetDto? ParameterSet { get; set; }

    // Selected ratings - SF
    public string SelectedSfPrec { get; set; } = "NOM";
    public string SelectedSfFlex { get; set; } = "NOM";
    public string SelectedSfResl { get; set; } = "NOM";
    public string SelectedSfTeam { get; set; } = "NOM";
    public string SelectedSfPmat { get; set; } = "NOM";

    // Selected ratings - EM (17 factors for COCOMO II Stage 3)
    public string SelectedEmRely { get; set; } = "NOM";
    public string SelectedEmData { get; set; } = "NOM";
    public string SelectedEmCplx { get; set; } = "NOM";
    public string SelectedEmRuse { get; set; } = "NOM";
    public string SelectedEmDocu { get; set; } = "NOM";
    public string SelectedEmTime { get; set; } = "NOM";
    public string SelectedEmStor { get; set; } = "NOM";
    public string SelectedEmPvol { get; set; } = "NOM";
    public string SelectedEmAcap { get; set; } = "NOM";
    public string SelectedEmPcap { get; set; } = "NOM";
    public string SelectedEmPcon { get; set; } = "NOM";
    public string SelectedEmApex { get; set; } = "NOM";
    public string SelectedEmPlex { get; set; } = "NOM";
    public string SelectedEmLtex { get; set; } = "NOM";
    public string SelectedEmTool { get; set; } = "NOM";
    public string SelectedEmSite { get; set; } = "NOM";
    public string SelectedEmSced { get; set; } = "NOM";

    // Calculated results
    public decimal? TotalUfp { get; set; }
    public decimal? Sloc { get; set; }
    public decimal? Ksloc { get; set; }
    public decimal? SumSf { get; set; }
    public decimal? ExponentE { get; set; }
    public decimal? Eaf { get; set; }
    public decimal? EffortPm { get; set; }
    public decimal? TdevMonths { get; set; }
    public decimal? AvgTeamSize { get; set; }

    // Actual results (post-mortem)
    public decimal? ActualEffortPm { get; set; }
    public decimal? ActualTdevMonths { get; set; }
    public decimal? ActualSloc { get; set; }

    // Functions (optional, for detailed view)
    public List<EstimationFunctionDto>? Functions { get; set; }
}