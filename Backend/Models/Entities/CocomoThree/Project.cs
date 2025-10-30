using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Entities;

namespace Backend.Models.Entities.CocomoThree;

/// <summary>
/// Project entity representing a COCOMO project container
/// </summary>
[Table("project")]
public class Project
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

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Estimation> Estimations { get; set; } = new List<Estimation>();
}
