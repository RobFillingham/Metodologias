using Backend.Models.Entities.CocomoTwoStageOne;

namespace Backend.Repositories.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for ParameterSet repository operations (COCOMO 2 Stage 1)
/// </summary>
public interface IParameterSetCocomo2Stage1Repository
{
    Task<ParameterSetCocomo2Stage1?> GetByIdAsync(int paramSetId);
    Task<IEnumerable<ParameterSetCocomo2Stage1>> GetByUserIdAsync(int userId);
    Task<IEnumerable<ParameterSetCocomo2Stage1>> GetDefaultSetsAsync();
    Task<ParameterSetCocomo2Stage1> CreateAsync(ParameterSetCocomo2Stage1 parameterSet);
    Task<ParameterSetCocomo2Stage1> UpdateAsync(ParameterSetCocomo2Stage1 parameterSet);
    Task<bool> DeleteAsync(int paramSetId);
    Task<bool> ExistsAsync(int paramSetId);
}
