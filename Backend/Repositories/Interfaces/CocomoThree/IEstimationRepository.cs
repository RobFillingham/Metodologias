using Backend.Models.Entities.CocomoThree;

namespace Backend.Repositories.Interfaces.CocomoThree;

/// <summary>
/// Repository interface for Estimation entity operations
/// </summary>
public interface IEstimationRepository
{
    /// <summary>
    /// Get all estimations for a specific project
    /// </summary>
    Task<IEnumerable<Estimation>> GetByProjectIdAsync(int projectId);

    /// <summary>
    /// Get estimation by ID
    /// </summary>
    Task<Estimation?> GetByIdAsync(int estimationId);

    /// <summary>
    /// Get estimation by ID with all related data (functions, parameter set, language)
    /// </summary>
    Task<Estimation?> GetByIdWithDetailsAsync(int estimationId);

    /// <summary>
    /// Create a new estimation
    /// </summary>
    Task<Estimation> CreateAsync(Estimation estimation);

    /// <summary>
    /// Update an existing estimation
    /// </summary>
    Task<Estimation> UpdateAsync(Estimation estimation);

    /// <summary>
    /// Delete an estimation
    /// </summary>
    Task<bool> DeleteAsync(int estimationId);

    /// <summary>
    /// Check if estimation belongs to project
    /// </summary>
    Task<bool> BelongsToProjectAsync(int estimationId, int projectId);
}
