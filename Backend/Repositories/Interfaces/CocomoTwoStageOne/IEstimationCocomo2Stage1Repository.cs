using Backend.Models.Entities.CocomoTwoStageOne;

namespace Backend.Repositories.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for Estimation repository operations (COCOMO 2 Stage 1)
/// </summary>
public interface IEstimationCocomo2Stage1Repository
{
    Task<EstimationCocomo2Stage1?> GetByIdAsync(int estimationId);
    Task<EstimationCocomo2Stage1?> GetByIdWithDetailsAsync(int estimationId);
    Task<IEnumerable<EstimationCocomo2Stage1>> GetByProjectIdAsync(int projectId);
    Task<EstimationCocomo2Stage1> CreateAsync(EstimationCocomo2Stage1 estimation);
    Task<EstimationCocomo2Stage1> UpdateAsync(EstimationCocomo2Stage1 estimation);
    Task<bool> DeleteAsync(int estimationId);
}
