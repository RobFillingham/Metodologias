using Backend.Data.Context;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations;

/// <summary>
/// Repository implementation for KLOC Estimation operations
/// </summary>
public class KlocEstimationRepository : IKlocEstimationRepository
{
    private readonly ApplicationDbContext _context;

    public KlocEstimationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<KlocEstimation>> GetEstimationsByProjectAsync(int projectId)
    {
        return await _context.KlocEstimations
            .Where(e => e.ProjectId == projectId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<KlocEstimation?> GetEstimationByIdAsync(int estimationId)
    {
        return await _context.KlocEstimations
            .FirstOrDefaultAsync(e => e.KlocEstimationId == estimationId);
    }

    public async Task<KlocEstimation> CreateEstimationAsync(KlocEstimation estimation)
    {
        estimation.CreatedAt = DateTime.UtcNow;
        estimation.UpdatedAt = DateTime.UtcNow;

        _context.KlocEstimations.Add(estimation);
        await _context.SaveChangesAsync();

        return estimation;
    }

    public async Task<KlocEstimation> UpdateEstimationAsync(KlocEstimation estimation)
    {
        estimation.UpdatedAt = DateTime.UtcNow;

        _context.KlocEstimations.Update(estimation);
        await _context.SaveChangesAsync();

        return estimation;
    }

    public async Task<bool> DeleteEstimationAsync(int estimationId)
    {
        var estimation = await _context.KlocEstimations.FindAsync(estimationId);
        if (estimation == null)
            return false;

        _context.KlocEstimations.Remove(estimation);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EstimationExistsAsync(int estimationId)
    {
        return await _context.KlocEstimations
            .AnyAsync(e => e.KlocEstimationId == estimationId);
    }
}
