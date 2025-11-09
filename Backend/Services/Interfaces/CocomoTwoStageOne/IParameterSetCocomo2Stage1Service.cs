using Backend.Models.DTOs.CocomoTwoStageOne;

namespace Backend.Services.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for ParameterSet service operations (COCOMO 2 Stage 1)
/// </summary>
public interface IParameterSetCocomo2Stage1Service
{
    Task<ParameterSetCocomo2Stage1Dto?> GetByIdAsync(int paramSetId, int userId);
    Task<IEnumerable<ParameterSetCocomo2Stage1Dto>> GetUserSetsAsync(int userId);
    Task<IEnumerable<ParameterSetCocomo2Stage1Dto>> GetDefaultSetsAsync();
    Task<ParameterSetCocomo2Stage1Dto> CreateAsync(CreateParameterSetCocomo2Stage1Dto createDto, int userId);
    Task<ParameterSetCocomo2Stage1Dto> UpdateAsync(int paramSetId, CreateParameterSetCocomo2Stage1Dto updateDto, int userId);
    Task<bool> DeleteAsync(int paramSetId, int userId);
}
