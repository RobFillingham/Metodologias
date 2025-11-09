using Backend.Models.DTOs.CocomoIIStage3;

namespace Backend.Services.Interfaces.CocomoIIStage3;

/// <summary>
/// Service interface for Estimation operations
/// </summary>
public interface IEstimationService
{
    /// <summary>
    /// Get all estimations for a specific project
    /// </summary>
    Task<IEnumerable<EstimationDto>> GetProjectEstimationsAsync(int projectId, int userId);

    /// <summary>
    /// Get estimation by ID with all details
    /// </summary>
    Task<EstimationDto?> GetEstimationByIdAsync(int estimationId, int userId);

    /// <summary>
    /// Create a new estimation
    /// </summary>
    Task<EstimationDto> CreateEstimationAsync(int projectId, CreateEstimationDto createDto, int userId);

    /// <summary>
    /// Update estimation ratings and recalculate
    /// </summary>
    Task<EstimationDto> UpdateEstimationRatingsAsync(int estimationId, UpdateEstimationRatingsDto updateDto, int userId);

    /// <summary>
    /// Update actual results (post-mortem)
    /// </summary>
    Task<EstimationDto> UpdateActualResultsAsync(int estimationId, UpdateActualResultsDto updateDto, int userId);

    /// <summary>
    /// Delete an estimation
    /// </summary>
    Task<bool> DeleteEstimationAsync(int estimationId, int userId);
}