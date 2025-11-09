using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Entities.CocomoThree;

namespace Backend.Models.Entities.CocomoTwoStageOne;

/// <summary>
/// Estimation entity for COCOMO 2 Stage 1 (Composition Model)
/// Stores a snapshot of a specific calculation
/// </summary>
[Table("EstimationCocomo2Stage1")]
public class EstimationCocomo2Stage1
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

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- SELECTED RATINGS (ACEMs) ---
    [Column("selected_aexp")]
    public string SelectedAexp { get; set; } = "nominal";

    [Column("selected_pexp")]
    public string SelectedPexp { get; set; } = "nominal";

    [Column("selected_prec")]
    public string SelectedPrec { get; set; } = "nominal";

    [Column("selected_rely")]
    public string SelectedRely { get; set; } = "nominal";

    [Column("selected_tmsp")]
    public string SelectedTmsp { get; set; } = "nominal";

    // --- CALCULATED RESULTS ---
    [Column("total_fp")]
    public decimal? TotalFp { get; set; }

    [Column("sloc")]
    public decimal? Sloc { get; set; }

    [Column("ksloc")]
    public decimal? Ksloc { get; set; }

    // Individual multipliers
    [Column("aexp_multiplier")]
    public decimal? AexpMultiplier { get; set; }

    [Column("pexp_multiplier")]
    public decimal? PexpMultiplier { get; set; }

    [Column("prec_multiplier")]
    public decimal? PrecMultiplier { get; set; }

    [Column("rely_multiplier")]
    public decimal? RelyMultiplier { get; set; }

    [Column("tmsp_multiplier")]
    public decimal? TmspMultiplier { get; set; }

    // EAF: Effort Adjustment Factor
    [Column("eaf")]
    public decimal? Eaf { get; set; }

    // Estimated effort
    [Column("effort_pm")]
    public decimal? EffortPm { get; set; }

    [Column("effort_hours")]
    public decimal? EffortHours { get; set; }

    // --- ACTUAL RESULTS (Post-mortem) ---
    [Column("actual_effort_pm")]
    public decimal? ActualEffortPm { get; set; }

    [Column("actual_effort_hours")]
    public decimal? ActualEffortHours { get; set; }

    [Column("actual_sloc")]
    public decimal? ActualSloc { get; set; }

    // Navigation properties
    public ProjectCocomo2Stage1? Project { get; set; }
    public ParameterSetCocomo2Stage1? ParameterSet { get; set; }
    public Language? Language { get; set; }
    public ICollection<EstimationComponentCocomo2Stage1> Components { get; set; } = new List<EstimationComponentCocomo2Stage1>();
}
