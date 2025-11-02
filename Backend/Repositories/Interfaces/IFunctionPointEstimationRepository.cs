using Backend.Models.Entities;

namespace Backend.Repositories.Interfaces;

/// <summary>
/// Interface for Function Point Estimation repository operations
/// </summary>
public interface IFunctionPointEstimationRepository
{
    /// <summary>
    /// Get all Function Point estimations for a specific project
    /// </summary>
    Task<IEnumerable<FunctionPointEstimation>> GetEstimationsByProjectAsync(int projectId);

    /// <summary>
    /// Get a specific Function Point estimation by ID
    /// </summary>
    Task<FunctionPointEstimation?> GetEstimationByIdAsync(int estimationId);

    /// <summary>
    /// Create a new Function Point estimation
    /// </summary>
    Task<FunctionPointEstimation> CreateEstimationAsync(FunctionPointEstimation estimation);

    /// <summary>
    /// Update an existing Function Point estimation
    /// </summary>
    Task<FunctionPointEstimation> UpdateEstimationAsync(FunctionPointEstimation estimation);

    /// <summary>
    /// Delete a Function Point estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId);

    /// <summary>
    /// Check if an estimation exists
    /// </summary>
    Task<bool> EstimationExistsAsync(int estimationId);
}
