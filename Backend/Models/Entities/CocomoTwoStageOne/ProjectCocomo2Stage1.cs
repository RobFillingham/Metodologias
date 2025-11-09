using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Entities;

namespace Backend.Models.Entities.CocomoTwoStageOne;

/// <summary>
/// Project entity for COCOMO 2 Stage 1
/// </summary>
[Table("ProjectCocomo2Stage1")]
public class ProjectCocomo2Stage1
{
    [Column("project_id")]
    public int ProjectId { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    [Column("project_name")]
    public string ProjectName { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public ICollection<EstimationCocomo2Stage1> Estimations { get; set; } = new List<EstimationCocomo2Stage1>();
}
