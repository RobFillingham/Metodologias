using Backend.Models.DTOs.CocomoIIStage3;

namespace Backend.Services.Interfaces.CocomoIIStage3;

/// <summary>
/// Service interface for EstimationFunction operations
/// </summary>
public interface IEstimationFunctionService
{
    /// <summary>
    /// Get all functions for an estimation
    /// </summary>
    Task<IEnumerable<EstimationFunctionDto>> GetEstimationFunctionsAsync(int estimationId, int userId);

    /// <summary>
    /// Get function by ID
    /// </summary>
    Task<EstimationFunctionDto?> GetFunctionByIdAsync(int functionId, int userId);

    /// <summary>
    /// Create a new function (calculates complexity and recalculates estimation)
    /// </summary>
    Task<EstimationFunctionDto> CreateFunctionAsync(int estimationId, CreateEstimationFunctionDto createDto, int userId);

    /// <summary>
    /// Create multiple functions in batch (calculates complexity for all and recalculates estimation once)
    /// </summary>
    Task<IEnumerable<EstimationFunctionDto>> CreateBatchFunctionsAsync(int estimationId, CreateBatchEstimationFunctionsDto batchDto, int userId);

    /// <summary>
    /// Update a function (recalculates complexity and recalculates estimation)
    /// </summary>
    Task<EstimationFunctionDto> UpdateFunctionAsync(int functionId, UpdateEstimationFunctionDto updateDto, int userId);

    /// <summary>
    /// Delete a function (recalculates estimation)
    /// </summary>
    Task<bool> DeleteFunctionAsync(int functionId, int userId);
}