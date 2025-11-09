using Backend.Models.DTOs.CocomoTwoStageOne;

namespace Backend.Services.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for Project service operations (COCOMO 2 Stage 1)
/// </summary>
public interface IProjectCocomo2Stage1Service
{
    Task<ProjectCocomo2Stage1Dto?> GetByIdAsync(int projectId, int userId);
    Task<IEnumerable<ProjectCocomo2Stage1Dto>> GetUserProjectsAsync(int userId);
    Task<ProjectCocomo2Stage1Dto> CreateAsync(CreateProjectCocomo2Stage1Dto createDto, int userId);
    Task<ProjectCocomo2Stage1Dto> UpdateAsync(int projectId, UpdateProjectCocomo2Stage1Dto updateDto, int userId);
    Task<bool> DeleteAsync(int projectId, int userId);
}
