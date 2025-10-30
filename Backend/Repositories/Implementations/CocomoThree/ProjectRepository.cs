using Backend.Data.Context;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoThree;

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
        return await _context.Projects
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int projectId)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    public async Task<Project?> GetByIdWithEstimationsAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.Estimations)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int projectId)
    {
        var project = await GetByIdAsync(projectId);
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsOwnerAsync(int projectId, int userId)
    {
        return await _context.Projects
            .AnyAsync(p => p.ProjectId == projectId && p.UserId == userId);
    }
}
