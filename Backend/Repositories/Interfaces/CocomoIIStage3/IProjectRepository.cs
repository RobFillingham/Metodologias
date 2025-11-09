using Project = Backend.Models.Entities.CocomoThree.Project;

namespace Backend.Repositories.Interfaces.CocomoIIStage3;

/// <summary>
/// Repository interface for Project entity operations
/// Note: Uses the shared Project entity from CocomoThree
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Get all projects for a specific user
    /// </summary>
    Task<IEnumerable<Project>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Get project by ID
    /// </summary>
    Task<Project?> GetByIdAsync(int projectId);

    /// <summary>
    /// Get project by ID with estimations included
    /// </summary>
    Task<Project?> GetByIdWithEstimationsAsync(int projectId);

    /// <summary>
    /// Create a new project
    /// </summary>
    Task<Project> CreateAsync(Project project);

    /// <summary>
    /// Update an existing project
    /// </summary>
    Task<Project> UpdateAsync(Project project);

    /// <summary>
    /// Delete a project
    /// </summary>
    Task<bool> DeleteAsync(int projectId);

    /// <summary>
    /// Check if user owns the project
    /// </summary>
    Task<bool> IsOwnerAsync(int projectId, int userId);
}