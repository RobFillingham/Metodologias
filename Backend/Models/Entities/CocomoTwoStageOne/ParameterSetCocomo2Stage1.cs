using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Entities;

namespace Backend.Models.Entities.CocomoTwoStageOne;

/// <summary>
/// Parameter set entity for COCOMO 2 Stage 1 (Composition Model)
/// Stores effort multipliers applicable to this model
/// </summary>
[Table("ParameterSetCocomo2Stage1")]
public class ParameterSetCocomo2Stage1
{
    [Column("param_set_id")]
    public int ParamSetId { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; }

    [Column("set_name")]
    public string SetName { get; set; } = string.Empty;

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- COCOMO 2 Composition Model Constants ---
    [Column("const_A")]
    public decimal ConstA { get; set; } = 2.45m;

    [Column("const_B")]
    public decimal ConstB { get; set; } = 1.10m;

    // --- AEXP: Application Experience ---
    [Column("aexp_very_low")]
    public decimal? AexpVeryLow { get; set; }

    [Column("aexp_low")]
    public decimal? AexpLow { get; set; }

    [Column("aexp_nominal")]
    public decimal? AexpNominal { get; set; }

    [Column("aexp_high")]
    public decimal? AexpHigh { get; set; }

    // --- PEXP: Platform Experience ---
    [Column("pexp_very_low")]
    public decimal? PexpVeryLow { get; set; }

    [Column("pexp_low")]
    public decimal? PexpLow { get; set; }

    [Column("pexp_nominal")]
    public decimal? PexpNominal { get; set; }

    [Column("pexp_high")]
    public decimal? PexpHigh { get; set; }

    // --- PREC: Precedentedness ---
    [Column("prec_very_low")]
    public decimal? PrecVeryLow { get; set; }

    [Column("prec_low")]
    public decimal? PrecLow { get; set; }

    [Column("prec_nominal")]
    public decimal? PrecNominal { get; set; }

    [Column("prec_high")]
    public decimal? PrecHigh { get; set; }

    // --- RELY: Required Reliability ---
    [Column("rely_low")]
    public decimal? RelyLow { get; set; }

    [Column("rely_nominal")]
    public decimal? RelyNominal { get; set; }

    [Column("rely_high")]
    public decimal? RelyHigh { get; set; }

    // --- TMSP: Time to Market Pressure ---
    [Column("tmsp_low")]
    public decimal? TmspLow { get; set; }

    [Column("tmsp_nominal")]
    public decimal? TmspNominal { get; set; }

    [Column("tmsp_high")]
    public decimal? TmspHigh { get; set; }

    // Navigation property
    public User? User { get; set; }
}
