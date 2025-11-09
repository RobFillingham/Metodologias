using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Services.Implementations.CocomoIIStage3;

/// <summary>
/// Service implementation for Project operations
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId)
    {
        var projects = await _projectRepository.GetByUserIdAsync(userId);

        return projects.Select(p => MapToDto(p));
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int projectId, int userId)
    {
        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(projectId, userId))
        {
            return null;
        }

        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project == null)
            return null;

        return MapToDto(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, int userId)
    {
        var project = new Project
        {
            UserId = userId,
            ProjectName = createDto.ProjectName,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        var createdProject = await _projectRepository.CreateAsync(project);
        return MapToDto(createdProject);
    }

    public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto updateDto, int userId)
    {
        var existingProject = await _projectRepository.GetByIdAsync(updateDto.ProjectId);

        if (existingProject == null)
            throw new KeyNotFoundException($"Project with ID {updateDto.ProjectId} not found");

        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(updateDto.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this project");
        }

        existingProject.ProjectName = updateDto.ProjectName;
        existingProject.Description = updateDto.Description;

        var updatedProject = await _projectRepository.UpdateAsync(existingProject);
        return MapToDto(updatedProject);
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this project");
        }

        return await _projectRepository.DeleteAsync(projectId);
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            ProjectId = project.ProjectId,
            UserId = project.UserId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            CreatedAt = project.CreatedAt
        };
    }
}