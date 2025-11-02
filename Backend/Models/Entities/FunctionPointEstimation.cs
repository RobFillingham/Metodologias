using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities;

/// <summary>
/// Complexity levels for Function Point components
/// </summary>
public enum FPComplexityLevel
{
    LOW,
    AVERAGE,
    HIGH
}

/// <summary>
/// Entity representing a Function Point (FP) estimation
/// </summary>
[Table("function_point_estimation")]
public class FunctionPointEstimation
{
    [Key]
    [Column("fp_estimation_id")]
    public int FpEstimationId { get; set; }

    [Column("project_id")]
    [Required]
    public int ProjectId { get; set; }

    [Column("estimation_name")]
    [Required]
    [StringLength(255)]
    public string EstimationName { get; set; } = string.Empty;

    [Column("external_inputs")]
    public int ExternalInputs { get; set; }

    [Column("external_outputs")]
    public int ExternalOutputs { get; set; }

    [Column("external_inquiries")]
    public int ExternalInquiries { get; set; }

    [Column("internal_logical_files")]
    public int InternalLogicalFiles { get; set; }

    [Column("external_interface_files")]
    public int ExternalInterfaceFiles { get; set; }

    [Column("complexity_level")]
    public FPComplexityLevel ComplexityLevel { get; set; } = FPComplexityLevel.AVERAGE;

    [Column("unadjusted_fp")]
    public decimal? UnadjustedFp { get; set; }

    [Column("value_adjustment_factor")]
    public decimal? ValueAdjustmentFactor { get; set; }

    [Column("adjusted_fp")]
    public decimal? AdjustedFp { get; set; }

    [Column("estimated_effort")]
    public decimal? EstimatedEffort { get; set; }

    [Column("estimated_cost")]
    public decimal? EstimatedCost { get; set; }

    [Column("estimated_time")]
    public decimal? EstimatedTime { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public ICollection<FunctionPointCharacteristic>? Characteristics { get; set; }
}

/// <summary>
/// Entity representing a technical characteristic for Function Point estimation
/// </summary>
[Table("function_point_characteristics")]
public class FunctionPointCharacteristic
{
    [Key]
    [Column("fp_char_id")]
    public int FpCharId { get; set; }

    [Column("fp_estimation_id")]
    [Required]
    public int FpEstimationId { get; set; }

    [Column("characteristic_name")]
    [Required]
    [StringLength(100)]
    public string CharacteristicName { get; set; } = string.Empty;

    [Column("influence_level")]
    public string InfluenceLevel { get; set; } = "AVERAGE";

    [Column("score")]
    public int Score { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public FunctionPointEstimation? FunctionPointEstimation { get; set; }
}
