using Backend.Models.DTOs;

namespace Backend.Services.Interfaces;

/// <summary>
/// Interface for KLOC Estimation service operations
/// </summary>
public interface IKlocEstimationService
{
    /// <summary>
    /// Get all KLOC estimations for a specific project
    /// </summary>
    Task<IEnumerable<KlocEstimationDto>> GetEstimationsByProjectAsync(int projectId);

    /// <summary>
    /// Get a specific KLOC estimation by ID
    /// </summary>
    Task<KlocEstimationDto?> GetEstimationByIdAsync(int estimationId);

    /// <summary>
    /// Create a new KLOC estimation with calculated metrics
    /// </summary>
    Task<KlocEstimationDto> CreateEstimationAsync(CreateKlocEstimationDto createDto);

    /// <summary>
    /// Update an existing KLOC estimation
    /// </summary>
    Task<KlocEstimationDto> UpdateEstimationAsync(UpdateKlocEstimationDto updateDto);

    /// <summary>
    /// Delete a KLOC estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId);
}
