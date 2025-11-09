using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoIIStage3;

/// <summary>
/// Estimation entity representing a COCOMO II Stage 3 calculation snapshot
/// </summary>
[Table("estimationcocomoIIstage3")]
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
    [Column("selected_em_rely")]
    public string SelectedEmRely { get; set; } = "NOM";

    [Column("selected_em_data")]
    public string SelectedEmData { get; set; } = "NOM";

    [Column("selected_em_cplx")]
    public string SelectedEmCplx { get; set; } = "NOM";

    [Column("selected_em_ruse")]
    public string SelectedEmRuse { get; set; } = "NOM";

    [Column("selected_em_docu")]
    public string SelectedEmDocu { get; set; } = "NOM";

    [Column("selected_em_time")]
    public string SelectedEmTime { get; set; } = "NOM";

    [Column("selected_em_stor")]
    public string SelectedEmStor { get; set; } = "NOM";

    [Column("selected_em_pvol")]
    public string SelectedEmPvol { get; set; } = "NOM";

    [Column("selected_em_acap")]
    public string SelectedEmAcap { get; set; } = "NOM";

    [Column("selected_em_pcap")]
    public string SelectedEmPcap { get; set; } = "NOM";

    [Column("selected_em_pcon")]
    public string SelectedEmPcon { get; set; } = "NOM";

    [Column("selected_em_apex")]
    public string SelectedEmApex { get; set; } = "NOM";

    [Column("selected_em_plex")]
    public string SelectedEmPlex { get; set; } = "NOM";

    [Column("selected_em_ltex")]
    public string SelectedEmLtex { get; set; } = "NOM";

    [Column("selected_em_tool")]
    public string SelectedEmTool { get; set; } = "NOM";

    [Column("selected_em_site")]
    public string SelectedEmSite { get; set; } = "NOM";

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