using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for creating a new COCOMO II Stage 3 Project
/// </summary>
public class CreateProjectDto
{
    [Required(ErrorMessage = "Project name is required")]
    [StringLength(255, ErrorMessage = "Project name cannot exceed 255 characters")]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string? Description { get; set; }
}