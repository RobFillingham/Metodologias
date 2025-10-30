using Backend.Models.Entities.CocomoThree;

namespace Backend.Repositories.Interfaces.CocomoThree;

/// <summary>
/// Repository interface for EstimationFunction entity operations
/// </summary>
public interface IEstimationFunctionRepository
{
    /// <summary>
    /// Get all functions for a specific estimation
    /// </summary>
    Task<IEnumerable<EstimationFunction>> GetByEstimationIdAsync(int estimationId);

    /// <summary>
    /// Get function by ID
    /// </summary>
    Task<EstimationFunction?> GetByIdAsync(int functionId);

    /// <summary>
    /// Create a new function
    /// </summary>
    Task<EstimationFunction> CreateAsync(EstimationFunction function);

    /// <summary>
    /// Create multiple functions in batch
    /// </summary>
    Task<IEnumerable<EstimationFunction>> CreateBatchAsync(IEnumerable<EstimationFunction> functions);

    /// <summary>
    /// Update an existing function
    /// </summary>
    Task<EstimationFunction> UpdateAsync(EstimationFunction function);

    /// <summary>
    /// Delete a function
    /// </summary>
    Task<bool> DeleteAsync(int functionId);

    /// <summary>
    /// Check if function belongs to estimation
    /// </summary>
    Task<bool> BelongsToEstimationAsync(int functionId, int estimationId);

    /// <summary>
    /// Get total UFP for an estimation
    /// </summary>
    Task<decimal> GetTotalUfpAsync(int estimationId);
}
