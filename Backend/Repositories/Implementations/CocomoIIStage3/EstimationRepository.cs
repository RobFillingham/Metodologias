using Backend.Data.Context;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoIIStage3;

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
        return await _context.EstimationsCocomoIIStage3
            .Include(e => e.Language)
            .Include(e => e.ParameterSet)
            .Include(e => e.EstimationFunctions)
            .Where(e => e.ProjectId == projectId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Estimation?> GetByIdAsync(int estimationId)
    {
        return await _context.EstimationsCocomoIIStage3
            .FirstOrDefaultAsync(e => e.EstimationId == estimationId);
    }

    public async Task<Estimation?> GetByIdWithDetailsAsync(int estimationId)
    {
        return await _context.EstimationsCocomoIIStage3
            .Include(e => e.EstimationFunctions)
            .Include(e => e.ParameterSet)
            .Include(e => e.Language)
            .Include(e => e.Project)
            .FirstOrDefaultAsync(e => e.EstimationId == estimationId);
    }

    public async Task<Estimation> CreateAsync(Estimation estimation)
    {
        _context.EstimationsCocomoIIStage3.Add(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<Estimation> UpdateAsync(Estimation estimation)
    {
        _context.EstimationsCocomoIIStage3.Update(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<bool> DeleteAsync(int estimationId)
    {
        var estimation = await GetByIdAsync(estimationId);
        if (estimation == null)
            return false;

        _context.EstimationsCocomoIIStage3.Remove(estimation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BelongsToProjectAsync(int estimationId, int projectId)
    {
        return await _context.EstimationsCocomoIIStage3
            .AnyAsync(e => e.EstimationId == estimationId && e.ProjectId == projectId);
    }

    public async Task<IEnumerable<Estimation>> GetByParameterSetIdAsync(int paramSetId)
    {
        return await _context.EstimationsCocomoIIStage3
            .Where(e => e.ParamSetId == paramSetId)
            .ToListAsync();
    }
}