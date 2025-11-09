using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Backend.Services.Interfaces.CocomoTwoStageOne;

namespace Backend.Services.Implementations.CocomoTwoStageOne;

/// <summary>
/// Service implementation for Project (COCOMO 2 Stage 1)
/// </summary>
public class ProjectCocomo2Stage1Service : IProjectCocomo2Stage1Service
{
    private readonly IProjectCocomo2Stage1Repository _repository;

    public ProjectCocomo2Stage1Service(IProjectCocomo2Stage1Repository repository)
    {
        _repository = repository;
    }

    public async Task<ProjectCocomo2Stage1Dto?> GetByIdAsync(int projectId, int userId)
    {
        // Check ownership
        if (!await _repository.IsOwnerAsync(projectId, userId))
        {
            return null;
        }

        var project = await _repository.GetByIdAsync(projectId);
        return project == null ? null : MapToDto(project);
    }

    public async Task<IEnumerable<ProjectCocomo2Stage1Dto>> GetUserProjectsAsync(int userId)
    {
        var projects = await _repository.GetByUserIdAsync(userId);
        return projects.Select(MapToDto);
    }

    public async Task<ProjectCocomo2Stage1Dto> CreateAsync(CreateProjectCocomo2Stage1Dto createDto, int userId)
    {
        var project = new ProjectCocomo2Stage1
        {
            UserId = userId,
            ProjectName = createDto.ProjectName,
            Description = createDto.Description
        };

        var created = await _repository.CreateAsync(project);
        return MapToDto(created);
    }

    public async Task<ProjectCocomo2Stage1Dto> UpdateAsync(int projectId, UpdateProjectCocomo2Stage1Dto updateDto, int userId)
    {
        // Check ownership
        if (!await _repository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this project");
        }

        var project = await _repository.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {projectId} not found");
        }

        // Update properties
        project.ProjectName = updateDto.ProjectName;
        project.Description = updateDto.Description;

        var updated = await _repository.UpdateAsync(project);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int projectId, int userId)
    {
        // Check ownership
        if (!await _repository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this project");
        }

        return await _repository.DeleteAsync(projectId);
    }

    private static ProjectCocomo2Stage1Dto MapToDto(ProjectCocomo2Stage1 project)
    {
        return new ProjectCocomo2Stage1Dto
        {
            ProjectId = project.ProjectId,
            UserId = project.UserId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
