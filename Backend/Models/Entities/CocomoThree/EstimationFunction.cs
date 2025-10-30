using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoThree;

/// <summary>
/// EstimationFunction entity representing function point inputs
/// </summary>
[Table("estimationfunction")]
public class EstimationFunction
{
    [Column("function_id")]
    public int FunctionId { get; set; }

    [Column("estimation_id")]
    public int EstimationId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("type")]
    public string Type { get; set; } = string.Empty; // 'EI', 'EO', 'EQ', 'ILF', 'EIF'

    [Column("det")]
    public int? Det { get; set; } // Data Element Types

    [Column("ret_ftr")]
    public int? RetFtr { get; set; } // Record Element Types / File Types Referenced

    [Column("complexity")]
    public string? Complexity { get; set; } // "Baja", "Media", "Alta"

    [Column("calculated_points")]
    public decimal? CalculatedPoints { get; set; }

    // Navigation property
    public Estimation? Estimation { get; set; }
}
