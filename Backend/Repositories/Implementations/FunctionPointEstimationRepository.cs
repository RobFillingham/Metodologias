using Backend.Data.Context;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations;

/// <summary>
/// Repository implementation for Function Point Estimation operations
/// </summary>
public class FunctionPointEstimationRepository : IFunctionPointEstimationRepository
{
    private readonly ApplicationDbContext _context;

    public FunctionPointEstimationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FunctionPointEstimation>> GetEstimationsByProjectAsync(int projectId)
    {
        return await _context.FunctionPointEstimations
            .Where(e => e.ProjectId == projectId)
            .Include(e => e.Characteristics)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<FunctionPointEstimation?> GetEstimationByIdAsync(int estimationId)
    {
        return await _context.FunctionPointEstimations
            .Include(e => e.Characteristics)
            .FirstOrDefaultAsync(e => e.FpEstimationId == estimationId);
    }

    public async Task<FunctionPointEstimation> CreateEstimationAsync(FunctionPointEstimation estimation)
    {
        estimation.CreatedAt = DateTime.UtcNow;
        estimation.UpdatedAt = DateTime.UtcNow;

        _context.FunctionPointEstimations.Add(estimation);
        await _context.SaveChangesAsync();

        return estimation;
    }

    public async Task<FunctionPointEstimation> UpdateEstimationAsync(FunctionPointEstimation estimation)
    {
        estimation.UpdatedAt = DateTime.UtcNow;

        _context.FunctionPointEstimations.Update(estimation);
        await _context.SaveChangesAsync();

        return estimation;
    }

    public async Task<bool> DeleteEstimationAsync(int estimationId)
    {
        var estimation = await _context.FunctionPointEstimations.FindAsync(estimationId);
        if (estimation == null)
            return false;

        _context.FunctionPointEstimations.Remove(estimation);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EstimationExistsAsync(int estimationId)
    {
        return await _context.FunctionPointEstimations
            .AnyAsync(e => e.FpEstimationId == estimationId);
    }
}
