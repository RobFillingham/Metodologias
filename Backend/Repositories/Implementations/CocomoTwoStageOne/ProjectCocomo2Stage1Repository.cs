using Backend.Data.Context;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoTwoStageOne;

/// <summary>
/// Repository implementation for Project (COCOMO 2 Stage 1)
/// </summary>
public class ProjectCocomo2Stage1Repository : IProjectCocomo2Stage1Repository
{
    private readonly ApplicationDbContext _context;

    public ProjectCocomo2Stage1Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectCocomo2Stage1?> GetByIdAsync(int projectId)
    {
        return await _context.ProjectsCocomo2Stage1
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    public async Task<IEnumerable<ProjectCocomo2Stage1>> GetByUserIdAsync(int userId)
    {
        return await _context.ProjectsCocomo2Stage1
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<ProjectCocomo2Stage1> CreateAsync(ProjectCocomo2Stage1 project)
    {
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        _context.ProjectsCocomo2Stage1.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<ProjectCocomo2Stage1> UpdateAsync(ProjectCocomo2Stage1 project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        _context.ProjectsCocomo2Stage1.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int projectId)
    {
        var project = await _context.ProjectsCocomo2Stage1
            .FindAsync(projectId);

        if (project == null)
            return false;

        _context.ProjectsCocomo2Stage1.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsOwnerAsync(int projectId, int userId)
    {
        return await _context.ProjectsCocomo2Stage1
            .AnyAsync(p => p.ProjectId == projectId && p.UserId == userId);
    }
}
