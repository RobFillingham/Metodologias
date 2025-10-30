using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoThree;

/// <summary>
/// Estimation entity representing a COCOMO II calculation snapshot
/// </summary>
[Table("estimation")]
public class Estimation
{
    [Column("estimation_id")]
    public int EstimationId { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [Column("param_set_id")]
    public int ParamSetId { get; set; }

    [Column("language_id")]
    public int LanguageId { get; set; }

    [Column("estimation_name")]
    public string EstimationName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- SELECTED RATINGS ---
    // Scale Factors (SF) - User selections
    [Column("selected_sf_prec")]
    public string SelectedSfPrec { get; set; } = "NOM";

    [Column("selected_sf_flex")]
    public string SelectedSfFlex { get; set; } = "NOM";

    [Column("selected_sf_resl")]
    public string SelectedSfResl { get; set; } = "NOM";

    [Column("selected_sf_team")]
    public string SelectedSfTeam { get; set; } = "NOM";

    [Column("selected_sf_pmat")]
    public string SelectedSfPmat { get; set; } = "NOM";

    // Effort Multipliers (EM) - User selections
    [Column("selected_em_pers")]
    public string SelectedEmPers { get; set; } = "NOM";

    [Column("selected_em_rcpx")]
    public string SelectedEmRcpx { get; set; } = "NOM";

    [Column("selected_em_pdif")]
    public string SelectedEmPdif { get; set; } = "NOM";

    [Column("selected_em_prex")]
    public string SelectedEmPrex { get; set; } = "NOM";

    [Column("selected_em_ruse")]
    public string SelectedEmRuse { get; set; } = "NOM";

    [Column("selected_em_fcil")]
    public string SelectedEmFcil { get; set; } = "NOM";

    [Column("selected_em_sced")]
    public string SelectedEmSced { get; set; } = "NOM";

    // --- CALCULATED RESULTS ---
    [Column("total_ufp")]
    public decimal? TotalUfp { get; set; }

    [Column("sloc")]
    public decimal? Sloc { get; set; }

    [Column("ksloc")]
    public decimal? Ksloc { get; set; }

    [Column("sum_sf")]
    public decimal? SumSf { get; set; }

    [Column("exponent_E")]
    public decimal? ExponentE { get; set; }

    [Column("eaf")]
    public decimal? Eaf { get; set; }

    [Column("effort_pm")]
    public decimal? EffortPm { get; set; }

    [Column("tdev_months")]
    public decimal? TdevMonths { get; set; }

    [Column("avg_team_size")]
    public decimal? AvgTeamSize { get; set; }

    // --- ACTUAL RESULTS (Post-mortem) ---
    [Column("actual_effort_pm")]
    public decimal? ActualEffortPm { get; set; }

    [Column("actual_tdev_months")]
    public decimal? ActualTdevMonths { get; set; }

    [Column("actual_sloc")]
    public decimal? ActualSloc { get; set; }

    // Navigation properties
    public Project? Project { get; set; }
    public ParameterSet? ParameterSet { get; set; }
    public Language? Language { get; set; }
    public ICollection<EstimationFunction> EstimationFunctions { get; set; } = new List<EstimationFunction>();
}
