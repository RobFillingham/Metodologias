using Backend.Models.DTOs.CocomoTwoStageOne;

namespace Backend.Services.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for Estimation service operations (COCOMO 2 Stage 1)
/// </summary>
public interface IEstimationCocomo2Stage1Service
{
    Task<EstimationCocomo2Stage1Dto?> GetByIdAsync(int estimationId, int userId);
    Task<IEnumerable<EstimationCocomo2Stage1Dto>> GetProjectEstimationsAsync(int projectId, int userId);
    Task<EstimationCocomo2Stage1Dto> CreateAsync(int projectId, CreateEstimationCocomo2Stage1Dto createDto, int userId);
    Task<EstimationCocomo2Stage1Dto> UpdateRatingsAsync(int estimationId, UpdateRatingsCocomo2Stage1Dto updateDto, int userId);
    Task<EstimationCocomo2Stage1Dto> UpdateActualResultsAsync(int estimationId, UpdateActualResultsCocomo2Stage1Dto updateDto, int userId);
    Task<bool> DeleteAsync(int estimationId, int userId);
}
