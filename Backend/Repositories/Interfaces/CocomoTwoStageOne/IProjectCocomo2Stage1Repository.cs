using Backend.Models.Entities.CocomoTwoStageOne;

namespace Backend.Repositories.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for Project repository operations (COCOMO 2 Stage 1)
/// </summary>
public interface IProjectCocomo2Stage1Repository
{
    Task<ProjectCocomo2Stage1?> GetByIdAsync(int projectId);
    Task<IEnumerable<ProjectCocomo2Stage1>> GetByUserIdAsync(int userId);
    Task<ProjectCocomo2Stage1> CreateAsync(ProjectCocomo2Stage1 project);
    Task<ProjectCocomo2Stage1> UpdateAsync(ProjectCocomo2Stage1 project);
    Task<bool> DeleteAsync(int projectId);
    Task<bool> IsOwnerAsync(int projectId, int userId);
}
