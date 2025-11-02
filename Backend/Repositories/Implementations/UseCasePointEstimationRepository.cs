using Backend.Data.Context;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations;

/// <summary>
/// Repository implementation for Use Case Point Estimation operations
/// </summary>
public class UseCasePointEstimationRepository : IUseCasePointEstimationRepository
{
    private readonly ApplicationDbContext _context;

    public UseCasePointEstimationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UseCasePointEstimation>> GetEstimationsByProjectAsync(int projectId)
    {
        return await _context.UseCasePointEstimations
            .Where(e => e.ProjectId == projectId)
            .Include(e => e.TechnicalFactors)
            .Include(e => e.EnvironmentFactors)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<UseCasePointEstimation?> GetEstimationByIdAsync(int estimationId)
    {
        return await _context.UseCasePointEstimations
            .Include(e => e.TechnicalFactors)
            .Include(e => e.EnvironmentFactors)
            .FirstOrDefaultAsync(e => e.UcpEstimationId == estimationId);
    }

    public async Task<UseCasePointEstimation> CreateEstimationAsync(UseCasePointEstimation estimation)
    {
        estimation.CreatedAt = DateTime.UtcNow;
        estimation.UpdatedAt = DateTime.UtcNow;

        _context.UseCasePointEstimations.Add(estimation);
        await _context.SaveChangesAsync();

        return estimation;
    }

    public async Task<UseCasePointEstimation> UpdateEstimationAsync(UseCasePointEstimation estimation)
    {
        estimation.UpdatedAt = DateTime.UtcNow;

        _context.UseCasePointEstimations.Update(estimation);
        await _context.SaveChangesAsync();

        return estimation;
    }

    public async Task<bool> DeleteEstimationAsync(int estimationId)
    {
        var estimation = await _context.UseCasePointEstimations.FindAsync(estimationId);
        if (estimation == null)
            return false;

        _context.UseCasePointEstimations.Remove(estimation);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EstimationExistsAsync(int estimationId)
    {
        return await _context.UseCasePointEstimations
            .AnyAsync(e => e.UcpEstimationId == estimationId);
    }
}
