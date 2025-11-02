using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities;

/// <summary>
/// Entity representing a Use Case Point (UCP) estimation
/// </summary>
[Table("use_case_point_estimation")]
public class UseCasePointEstimation
{
    [Key]
    [Column("ucp_estimation_id")]
    public int UcpEstimationId { get; set; }

    [Column("project_id")]
    [Required]
    public int ProjectId { get; set; }

    [Column("estimation_name")]
    [Required]
    [StringLength(255)]
    public string EstimationName { get; set; } = string.Empty;

    [Column("simple_ucc_count")]
    public int SimpleUccCount { get; set; }

    [Column("average_ucc_count")]
    public int AverageUccCount { get; set; }

    [Column("complex_ucc_count")]
    public int ComplexUccCount { get; set; }

    [Column("simple_actor_count")]
    public int SimpleActorCount { get; set; }

    [Column("average_actor_count")]
    public int AverageActorCount { get; set; }

    [Column("complex_actor_count")]
    public int ComplexActorCount { get; set; }

    [Column("unadjusted_ucp")]
    public decimal? UnadjustedUcp { get; set; }

    [Column("technical_complexity_factor")]
    public decimal? TechnicalComplexityFactor { get; set; }

    [Column("environment_factor")]
    public decimal? EnvironmentFactor { get; set; }

    [Column("adjusted_ucp")]
    public decimal? AdjustedUcp { get; set; }

    [Column("estimated_effort")]
    public decimal? EstimatedEffort { get; set; }

    [Column("estimated_effort_pm")]
    public decimal? EstimatedEffortPm { get; set; }

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

    // Navigation properties
    public ICollection<UseCaseTechnicalFactor>? TechnicalFactors { get; set; }
    public ICollection<UseCaseEnvironmentFactor>? EnvironmentFactors { get; set; }
}

/// <summary>
/// Entity representing a technical factor for Use Case Point estimation
/// </summary>
[Table("use_case_technical_factors")]
public class UseCaseTechnicalFactor
{
    [Key]
    [Column("ucp_tech_factor_id")]
    public int UcpTechFactorId { get; set; }

    [Column("ucp_estimation_id")]
    [Required]
    public int UcpEstimationId { get; set; }

    [Column("factor_name")]
    [Required]
    [StringLength(100)]
    public string FactorName { get; set; } = string.Empty;

    [Column("factor_weight")]
    public int FactorWeight { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public UseCasePointEstimation? UseCasePointEstimation { get; set; }
}

/// <summary>
/// Entity representing an environment factor for Use Case Point estimation
/// </summary>
[Table("use_case_environment_factors")]
public class UseCaseEnvironmentFactor
{
    [Key]
    [Column("ucp_env_factor_id")]
    public int UcpEnvFactorId { get; set; }

    [Column("ucp_estimation_id")]
    [Required]
    public int UcpEstimationId { get; set; }

    [Column("factor_name")]
    [Required]
    [StringLength(100)]
    public string FactorName { get; set; } = string.Empty;

    [Column("factor_weight")]
    public int FactorWeight { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public UseCasePointEstimation? UseCasePointEstimation { get; set; }
}
