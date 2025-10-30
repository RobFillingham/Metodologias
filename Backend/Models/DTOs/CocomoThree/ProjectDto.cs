namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for Project entity
/// </summary>
public class ProjectDto
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
