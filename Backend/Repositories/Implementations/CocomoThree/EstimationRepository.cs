using Backend.Data.Context;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoThree;

/// <summary>
/// Repository implementation for Estimation entity operations
/// </summary>
public class EstimationRepository : IEstimationRepository
{
    private readonly ApplicationDbContext _context;

    public EstimationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Estimation>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Estimations
            .Where(e => e.ProjectId == projectId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Estimation?> GetByIdAsync(int estimationId)
    {
        return await _context.Estimations
            .FirstOrDefaultAsync(e => e.EstimationId == estimationId);
    }

    public async Task<Estimation?> GetByIdWithDetailsAsync(int estimationId)
    {
        return await _context.Estimations
            .Include(e => e.EstimationFunctions)
            .Include(e => e.ParameterSet)
            .Include(e => e.Language)
            .Include(e => e.Project)
            .FirstOrDefaultAsync(e => e.EstimationId == estimationId);
    }

    public async Task<Estimation> CreateAsync(Estimation estimation)
    {
        _context.Estimations.Add(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<Estimation> UpdateAsync(Estimation estimation)
    {
        _context.Estimations.Update(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<bool> DeleteAsync(int estimationId)
    {
        var estimation = await GetByIdAsync(estimationId);
        if (estimation == null)
            return false;

        _context.Estimations.Remove(estimation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BelongsToProjectAsync(int estimationId, int projectId)
    {
        return await _context.Estimations
            .AnyAsync(e => e.EstimationId == estimationId && e.ProjectId == projectId);
    }
}
