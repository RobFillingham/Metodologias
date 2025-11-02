using Backend.Models.DTOs;

namespace Backend.Services.Interfaces;

/// <summary>
/// Interface for Use Case Point Estimation service operations
/// </summary>
public interface IUseCasePointEstimationService
{
    /// <summary>
    /// Get all Use Case Point estimations for a specific project
    /// </summary>
    Task<IEnumerable<UseCasePointEstimationDto>> GetEstimationsByProjectAsync(int projectId);

    /// <summary>
    /// Get a specific Use Case Point estimation by ID
    /// </summary>
    Task<UseCasePointEstimationDto?> GetEstimationByIdAsync(int estimationId);

    /// <summary>
    /// Create a new Use Case Point estimation with calculated metrics
    /// </summary>
    Task<UseCasePointEstimationDto> CreateEstimationAsync(CreateUseCasePointEstimationDto createDto);

    /// <summary>
    /// Update an existing Use Case Point estimation
    /// </summary>
    Task<UseCasePointEstimationDto> UpdateEstimationAsync(UpdateUseCasePointEstimationDto updateDto);

    /// <summary>
    /// Delete a Use Case Point estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId);
}
