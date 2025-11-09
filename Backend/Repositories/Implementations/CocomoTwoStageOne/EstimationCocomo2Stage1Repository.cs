using Backend.Data.Context;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoTwoStageOne;

/// <summary>
/// Repository implementation for Estimation (COCOMO 2 Stage 1)
/// </summary>
public class EstimationCocomo2Stage1Repository : IEstimationCocomo2Stage1Repository
{
    private readonly ApplicationDbContext _context;

    public EstimationCocomo2Stage1Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EstimationCocomo2Stage1?> GetByIdAsync(int estimationId)
    {
        return await _context.EstimationsCocomo2Stage1
            .FindAsync(estimationId);
    }

    public async Task<EstimationCocomo2Stage1?> GetByIdWithDetailsAsync(int estimationId)
    {
        return await _context.EstimationsCocomo2Stage1
            .Include(e => e.Project)
            .Include(e => e.ParameterSet)
            .Include(e => e.Language)
            .Include(e => e.Components)
            .FirstOrDefaultAsync(e => e.EstimationId == estimationId);
    }

    public async Task<IEnumerable<EstimationCocomo2Stage1>> GetByProjectIdAsync(int projectId)
    {
        return await _context.EstimationsCocomo2Stage1
            .Include(e => e.Language)
            .Include(e => e.ParameterSet)
            .Where(e => e.ProjectId == projectId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<EstimationCocomo2Stage1> CreateAsync(EstimationCocomo2Stage1 estimation)
    {
        estimation.CreatedAt = DateTime.UtcNow;
        estimation.UpdatedAt = DateTime.UtcNow;
        _context.EstimationsCocomo2Stage1.Add(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<EstimationCocomo2Stage1> UpdateAsync(EstimationCocomo2Stage1 estimation)
    {
        estimation.UpdatedAt = DateTime.UtcNow;
        _context.EstimationsCocomo2Stage1.Update(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<bool> DeleteAsync(int estimationId)
    {
        var estimation = await _context.EstimationsCocomo2Stage1
            .FindAsync(estimationId);

        if (estimation == null)
            return false;

        _context.EstimationsCocomo2Stage1.Remove(estimation);
        await _context.SaveChangesAsync();
        return true;
    }
}
