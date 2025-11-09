using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoTwoStageOne;

/// <summary>
/// Component entity for COCOMO 2 Stage 1 estimations
/// Represents a reusable element (module, API, library)
/// </summary>
[Table("EstimationComponentCocomo2Stage1")]
public class EstimationComponentCocomo2Stage1
{
    [Column("component_id")]
    public int ComponentId { get; set; }

    [Column("estimation_id")]
    public int EstimationId { get; set; }

    [Column("component_name")]
    public string ComponentName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Component type: 'new', 'adapted', 'cots'
    /// </summary>
    [Column("component_type")]
    public string ComponentType { get; set; } = "new";

    /// <summary>
    /// Size in Function Points
    /// </summary>
    [Column("size_fp")]
    public decimal SizeFp { get; set; }

    /// <summary>
    /// Reuse percentage (0-100) for adapted or COTS components
    /// </summary>
    [Column("reuse_percent")]
    public int? ReusePercent { get; set; }

    /// <summary>
    /// Expected change percentage (0-100) for adapted components
    /// </summary>
    [Column("change_percent")]
    public int? ChangePercent { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public EstimationCocomo2Stage1? Estimation { get; set; }
}
