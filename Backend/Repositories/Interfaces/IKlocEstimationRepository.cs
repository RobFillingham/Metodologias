using Backend.Models.Entities;

namespace Backend.Repositories.Interfaces;

/// <summary>
/// Interface for KLOC Estimation repository operations
/// </summary>
public interface IKlocEstimationRepository
{
    /// <summary>
    /// Get all KLOC estimations for a specific project
    /// </summary>
    Task<IEnumerable<KlocEstimation>> GetEstimationsByProjectAsync(int projectId);

    /// <summary>
    /// Get a specific KLOC estimation by ID
    /// </summary>
    Task<KlocEstimation?> GetEstimationByIdAsync(int estimationId);

    /// <summary>
    /// Create a new KLOC estimation
    /// </summary>
    Task<KlocEstimation> CreateEstimationAsync(KlocEstimation estimation);

    /// <summary>
    /// Update an existing KLOC estimation
    /// </summary>
    Task<KlocEstimation> UpdateEstimationAsync(KlocEstimation estimation);

    /// <summary>
    /// Delete a KLOC estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId);

    /// <summary>
    /// Check if an estimation exists
    /// </summary>
    Task<bool> EstimationExistsAsync(int estimationId);
}
