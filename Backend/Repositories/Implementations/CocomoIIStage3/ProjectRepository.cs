using Backend.Data.Context;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoIIStage3;

/// <summary>
/// Repository implementation for Project entity operations
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetByUserIdAsync(int userId)
    {
        return await _context.ProjectsCocomoIIStage3
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int projectId)
    {
        return await _context.ProjectsCocomoIIStage3
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    public async Task<Project?> GetByIdWithEstimationsAsync(int projectId)
    {
        return await _context.ProjectsCocomoIIStage3
            .Include(p => p.Estimations)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.ProjectsCocomoIIStage3.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        _context.ProjectsCocomoIIStage3.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int projectId)
    {
        var project = await GetByIdAsync(projectId);
        if (project == null)
            return false;

        _context.ProjectsCocomoIIStage3.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsOwnerAsync(int projectId, int userId)
    {
        return await _context.ProjectsCocomoIIStage3
            .AnyAsync(p => p.ProjectId == projectId && p.UserId == userId);
    }
}