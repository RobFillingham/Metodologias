using Backend.Data.Context;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoTwoStageOne;

/// <summary>
/// Repository implementation for ParameterSet (COCOMO 2 Stage 1)
/// </summary>
public class ParameterSetCocomo2Stage1Repository : IParameterSetCocomo2Stage1Repository
{
    private readonly ApplicationDbContext _context;

    public ParameterSetCocomo2Stage1Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ParameterSetCocomo2Stage1?> GetByIdAsync(int paramSetId)
    {
        return await _context.ParameterSetsCocomo2Stage1
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ParamSetId == paramSetId);
    }

    public async Task<IEnumerable<ParameterSetCocomo2Stage1>> GetByUserIdAsync(int userId)
    {
        return await _context.ParameterSetsCocomo2Stage1
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParameterSetCocomo2Stage1>> GetDefaultSetsAsync()
    {
        return await _context.ParameterSetsCocomo2Stage1
            .Where(p => p.IsDefault == true)
            .OrderBy(p => p.SetName)
            .ToListAsync();
    }

    public async Task<ParameterSetCocomo2Stage1> CreateAsync(ParameterSetCocomo2Stage1 parameterSet)
    {
        parameterSet.CreatedAt = DateTime.UtcNow;
        _context.ParameterSetsCocomo2Stage1.Add(parameterSet);
        await _context.SaveChangesAsync();
        return parameterSet;
    }

    public async Task<ParameterSetCocomo2Stage1> UpdateAsync(ParameterSetCocomo2Stage1 parameterSet)
    {
        _context.ParameterSetsCocomo2Stage1.Update(parameterSet);
        await _context.SaveChangesAsync();
        return parameterSet;
    }

    public async Task<bool> DeleteAsync(int paramSetId)
    {
        var parameterSet = await _context.ParameterSetsCocomo2Stage1
            .FindAsync(paramSetId);

        if (parameterSet == null)
            return false;

        _context.ParameterSetsCocomo2Stage1.Remove(parameterSet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int paramSetId)
    {
        return await _context.ParameterSetsCocomo2Stage1
            .AnyAsync(p => p.ParamSetId == paramSetId);
    }
}
