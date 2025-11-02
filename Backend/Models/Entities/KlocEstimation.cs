using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities;

/// <summary>
/// Entity representing a KLOC (Lines of Code) estimation
/// </summary>
[Table("kloc_estimation")]
public class KlocEstimation
{
    [Key]
    [Column("kloc_estimation_id")]
    public int KlocEstimationId { get; set; }

    [Column("project_id")]
    [Required]
    public int ProjectId { get; set; }

    [Column("estimation_name")]
    [Required]
    [StringLength(255)]
    public string EstimationName { get; set; } = string.Empty;

    [Column("lines_of_code")]
    [Required]
    public int LinesOfCode { get; set; }

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
}
