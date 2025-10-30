using Backend.Data.Context;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoThree;

/// <summary>
/// Repository implementation for EstimationFunction entity operations
/// </summary>
public class EstimationFunctionRepository : IEstimationFunctionRepository
{
    private readonly ApplicationDbContext _context;

    public EstimationFunctionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EstimationFunction>> GetByEstimationIdAsync(int estimationId)
    {
        return await _context.EstimationFunctions
            .Where(f => f.EstimationId == estimationId)
            .OrderBy(f => f.FunctionId)
            .ToListAsync();
    }

    public async Task<EstimationFunction?> GetByIdAsync(int functionId)
    {
        return await _context.EstimationFunctions
            .FirstOrDefaultAsync(f => f.FunctionId == functionId);
    }

    public async Task<EstimationFunction> CreateAsync(EstimationFunction function)
    {
        _context.EstimationFunctions.Add(function);
        await _context.SaveChangesAsync();
        return function;
    }

    public async Task<IEnumerable<EstimationFunction>> CreateBatchAsync(IEnumerable<EstimationFunction> functions)
    {
        var functionsList = functions.ToList();
        _context.EstimationFunctions.AddRange(functionsList);
        await _context.SaveChangesAsync();
        return functionsList;
    }

    public async Task<EstimationFunction> UpdateAsync(EstimationFunction function)
    {
        _context.EstimationFunctions.Update(function);
        await _context.SaveChangesAsync();
        return function;
    }

    public async Task<bool> DeleteAsync(int functionId)
    {
        var function = await GetByIdAsync(functionId);
        if (function == null)
            return false;

        _context.EstimationFunctions.Remove(function);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BelongsToEstimationAsync(int functionId, int estimationId)
    {
        return await _context.EstimationFunctions
            .AnyAsync(f => f.FunctionId == functionId && f.EstimationId == estimationId);
    }

    public async Task<decimal> GetTotalUfpAsync(int estimationId)
    {
        var functions = await _context.EstimationFunctions
            .Where(f => f.EstimationId == estimationId)
            .ToListAsync();

        return functions.Sum(f => f.CalculatedPoints ?? 0);
    }
}
