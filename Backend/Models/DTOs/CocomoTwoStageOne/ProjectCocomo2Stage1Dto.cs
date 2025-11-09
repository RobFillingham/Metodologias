namespace Backend.Models.DTOs.CocomoTwoStageOne;

/// <summary>
/// DTO for Project (COCOMO 2 Stage 1)
/// </summary>
public class ProjectCocomo2Stage1Dto
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Project
/// </summary>
public class CreateProjectCocomo2Stage1Dto
{
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a Project
/// </summary>
public class UpdateProjectCocomo2Stage1Dto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
