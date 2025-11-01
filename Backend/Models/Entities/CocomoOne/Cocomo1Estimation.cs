using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoOne;

/// <summary>
/// Entidad para una estimación COCOMO 1 Estadio I (Básico)
/// </summary>
[Table("cocomo1_estimation")]
public class Cocomo1Estimation
{
    [Column("cocomo1_estimation_id")]
    public int Cocomo1EstimationId { get; set; }

    [Column("project_id")]
    public int ProjectId { get; set; }

    [Column("estimation_name")]
    public string EstimationName { get; set; } = string.Empty;

    [Column("kloc")]
    public decimal Kloc { get; set; }

    [Column("mode")]
    public string Mode { get; set; } = "ORGANIC"; // ORGANIC, SEMI_DETACHED, EMBEDDED

    [Column("effort_pm")]
    public decimal? EffortPm { get; set; }

    [Column("tdev_months")]
    public decimal? TdevMonths { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
