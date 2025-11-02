using Backend.Models.Entities;

namespace Backend.Repositories.Interfaces;

/// <summary>
/// Interface for Use Case Point Estimation repository operations
/// </summary>
public interface IUseCasePointEstimationRepository
{
    /// <summary>
    /// Get all Use Case Point estimations for a specific project
    /// </summary>
    Task<IEnumerable<UseCasePointEstimation>> GetEstimationsByProjectAsync(int projectId);

    /// <summary>
    /// Get a specific Use Case Point estimation by ID
    /// </summary>
    Task<UseCasePointEstimation?> GetEstimationByIdAsync(int estimationId);

    /// <summary>
    /// Create a new Use Case Point estimation
    /// </summary>
    Task<UseCasePointEstimation> CreateEstimationAsync(UseCasePointEstimation estimation);

    /// <summary>
    /// Update an existing Use Case Point estimation
    /// </summary>
    Task<UseCasePointEstimation> UpdateEstimationAsync(UseCasePointEstimation estimation);

    /// <summary>
    /// Delete a Use Case Point estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId);

    /// <summary>
    /// Check if an estimation exists
    /// </summary>
    Task<bool> EstimationExistsAsync(int estimationId);
}
