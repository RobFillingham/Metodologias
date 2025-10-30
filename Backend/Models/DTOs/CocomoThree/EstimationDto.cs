namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for Estimation entity with calculated results
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

    // Selected ratings - EM
    public string SelectedEmPers { get; set; } = "NOM";
    public string SelectedEmRcpx { get; set; } = "NOM";
    public string SelectedEmPdif { get; set; } = "NOM";
    public string SelectedEmPrex { get; set; } = "NOM";
    public string SelectedEmRuse { get; set; } = "NOM";
    public string SelectedEmFcil { get; set; } = "NOM";
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
