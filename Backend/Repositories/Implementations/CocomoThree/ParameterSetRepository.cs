using Backend.Data.Context;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoThree;

/// <summary>
/// Implementation of ParameterSet repository
/// </summary>
public class ParameterSetRepository : IParameterSetRepository
{
    private readonly ApplicationDbContext _context;

    public ParameterSetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ParameterSet>> GetByUserIdAsync(int userId)
    {
        return await _context.ParameterSets
            .Where(ps => ps.UserId == userId)
            .OrderBy(ps => ps.SetName)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParameterSet>> GetDefaultParameterSetsAsync()
    {
        return await _context.ParameterSets
            .Where(ps => ps.IsDefault && ps.UserId == null)
            .OrderBy(ps => ps.SetName)
            .ToListAsync();
    }

    public async Task<ParameterSet?> GetByIdAsync(int paramSetId)
    {
        return await _context.ParameterSets
            .FirstOrDefaultAsync(ps => ps.ParamSetId == paramSetId);
    }

    public async Task<ParameterSet> CreateAsync(ParameterSet parameterSet)
    {
        _context.ParameterSets.Add(parameterSet);
        await _context.SaveChangesAsync();
        return parameterSet;
    }

    public async Task<ParameterSet> UpdateAsync(ParameterSet parameterSet)
    {
        _context.ParameterSets.Update(parameterSet);
        await _context.SaveChangesAsync();
        return parameterSet;
    }

    public async Task<bool> DeleteAsync(int paramSetId)
    {
        var parameterSet = await GetByIdAsync(paramSetId);
        if (parameterSet == null)
            return false;

        _context.ParameterSets.Remove(parameterSet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int paramSetId)
    {
        return await _context.ParameterSets
            .AnyAsync(ps => ps.ParamSetId == paramSetId);
    }

    public async Task<bool> ExistsAndBelongsToUserAsync(int paramSetId, int userId)
    {
        return await _context.ParameterSets
            .AnyAsync(ps => ps.ParamSetId == paramSetId && ps.UserId == userId);
    }
}