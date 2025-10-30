using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for updating an existing Project
/// </summary>
public class UpdateProjectDto
{
    [Required]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [StringLength(255, ErrorMessage = "Project name cannot exceed 255 characters")]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string? Description { get; set; }
}
