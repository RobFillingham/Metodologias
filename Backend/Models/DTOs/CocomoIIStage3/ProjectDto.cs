namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for COCOMO II Stage 3 Project entity
/// </summary>
public class ProjectDto
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}