using Backend.Models.DTOs.CocomoThree;

namespace Backend.Services.Interfaces.CocomoThree;

/// <summary>
/// Service interface for Project operations
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Get all projects for a specific user
    /// </summary>
    Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId);

    /// <summary>
    /// Get project by ID
    /// </summary>
    Task<ProjectDto?> GetProjectByIdAsync(int projectId, int userId);

    /// <summary>
    /// Create a new project
    /// </summary>
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, int userId);

    /// <summary>
    /// Update an existing project
    /// </summary>
    Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updateDto, int userId);

    /// <summary>
    /// Delete a project
    /// </summary>
    Task<bool> DeleteProjectAsync(int projectId, int userId);
}
