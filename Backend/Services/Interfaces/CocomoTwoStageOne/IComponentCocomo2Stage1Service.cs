using Backend.Models.DTOs.CocomoTwoStageOne;

namespace Backend.Services.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for Component service operations (COCOMO 2 Stage 1)
/// </summary>
public interface IComponentCocomo2Stage1Service
{
    Task<ComponentCocomo2Stage1Dto?> GetByIdAsync(int componentId, int userId);
    Task<IEnumerable<ComponentCocomo2Stage1Dto>> GetEstimationComponentsAsync(int estimationId, int userId);
    Task<ComponentCocomo2Stage1Dto> CreateAsync(int estimationId, CreateComponentCocomo2Stage1Dto createDto, int userId);
    Task<IEnumerable<ComponentCocomo2Stage1Dto>> CreateBatchAsync(int estimationId, CreateBatchComponentsCocomo2Stage1Dto batchDto, int userId);
    Task<ComponentCocomo2Stage1Dto> UpdateAsync(int componentId, UpdateComponentCocomo2Stage1Dto updateDto, int userId);
    Task<bool> DeleteAsync(int componentId, int userId);
}
