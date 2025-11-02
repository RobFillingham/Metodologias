using Backend.Models.DTOs;

namespace Backend.Services.Interfaces;

/// <summary>
/// Interface for Function Point Estimation service operations
/// </summary>
public interface IFunctionPointEstimationService
{
    /// <summary>
    /// Get all Function Point estimations for a specific project
    /// </summary>
    Task<IEnumerable<FunctionPointEstimationDto>> GetEstimationsByProjectAsync(int projectId);

    /// <summary>
    /// Get a specific Function Point estimation by ID
    /// </summary>
    Task<FunctionPointEstimationDto?> GetEstimationByIdAsync(int estimationId);

    /// <summary>
    /// Create a new Function Point estimation with calculated metrics
    /// </summary>
    Task<FunctionPointEstimationDto> CreateEstimationAsync(CreateFunctionPointEstimationDto createDto);

    /// <summary>
    /// Update an existing Function Point estimation
    /// </summary>
    Task<FunctionPointEstimationDto> UpdateEstimationAsync(UpdateFunctionPointEstimationDto updateDto);

    /// <summary>
    /// Delete a Function Point estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId);
}
