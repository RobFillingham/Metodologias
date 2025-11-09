using Backend.Data.Context;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoIIStage3;

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
        return await _context.ParameterSetsCocomoIIStage3
            .Where(ps => ps.UserId == userId)
            .OrderBy(ps => ps.SetName)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParameterSet>> GetDefaultParameterSetsAsync()
    {
        return await _context.ParameterSetsCocomoIIStage3
            .Where(ps => ps.IsDefault && ps.UserId == null)
            .OrderBy(ps => ps.SetName)
            .ToListAsync();
    }

    public async Task<ParameterSet?> GetByIdAsync(int paramSetId)
    {
        return await _context.ParameterSetsCocomoIIStage3
            .FirstOrDefaultAsync(ps => ps.ParamSetId == paramSetId);
    }

    public async Task<ParameterSet> CreateAsync(ParameterSet parameterSet)
    {
        _context.ParameterSetsCocomoIIStage3.Add(parameterSet);
        await _context.SaveChangesAsync();
        return parameterSet;
    }

    public async Task<ParameterSet> UpdateAsync(ParameterSet parameterSet)
    {
        _context.ParameterSetsCocomoIIStage3.Update(parameterSet);
        await _context.SaveChangesAsync();
        return parameterSet;
    }

    public async Task<bool> DeleteAsync(int paramSetId)
    {
        var parameterSet = await GetByIdAsync(paramSetId);
        if (parameterSet == null)
            return false;

        _context.ParameterSetsCocomoIIStage3.Remove(parameterSet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int paramSetId)
    {
        return await _context.ParameterSetsCocomoIIStage3
            .AnyAsync(ps => ps.ParamSetId == paramSetId);
    }

    public async Task<bool> ExistsAndBelongsToUserAsync(int paramSetId, int userId)
    {
        return await _context.ParameterSetsCocomoIIStage3
            .AnyAsync(ps => ps.ParamSetId == paramSetId && ps.UserId == userId);
    }
}