using Backend.Models.Entities.CocomoTwoStageOne;

namespace Backend.Repositories.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for Component repository operations (COCOMO 2 Stage 1)
/// </summary>
public interface IComponentCocomo2Stage1Repository
{
    Task<EstimationComponentCocomo2Stage1?> GetByIdAsync(int componentId);
    Task<IEnumerable<EstimationComponentCocomo2Stage1>> GetByEstimationIdAsync(int estimationId);
    Task<EstimationComponentCocomo2Stage1> CreateAsync(EstimationComponentCocomo2Stage1 component);
    Task<IEnumerable<EstimationComponentCocomo2Stage1>> CreateBatchAsync(IEnumerable<EstimationComponentCocomo2Stage1> components);
    Task<EstimationComponentCocomo2Stage1> UpdateAsync(EstimationComponentCocomo2Stage1 component);
    Task<bool> DeleteAsync(int componentId);
    Task<decimal> GetTotalFpAsync(int estimationId);
}
