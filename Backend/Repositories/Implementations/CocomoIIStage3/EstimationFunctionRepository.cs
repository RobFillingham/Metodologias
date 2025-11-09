using Backend.Data.Context;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoIIStage3;

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
        return await _context.EstimationFunctionsCocomoIIStage3
            .Where(f => f.EstimationId == estimationId)
            .OrderBy(f => f.FunctionId)
            .ToListAsync();
    }

    public async Task<EstimationFunction?> GetByIdAsync(int functionId)
    {
        return await _context.EstimationFunctionsCocomoIIStage3
            .FirstOrDefaultAsync(f => f.FunctionId == functionId);
    }

    public async Task<EstimationFunction> CreateAsync(EstimationFunction function)
    {
        _context.EstimationFunctionsCocomoIIStage3.Add(function);
        await _context.SaveChangesAsync();
        return function;
    }

    public async Task<IEnumerable<EstimationFunction>> CreateBatchAsync(IEnumerable<EstimationFunction> functions)
    {
        var functionsList = functions.ToList();
        _context.EstimationFunctionsCocomoIIStage3.AddRange(functionsList);
        await _context.SaveChangesAsync();
        return functionsList;
    }

    public async Task<EstimationFunction> UpdateAsync(EstimationFunction function)
    {
        _context.EstimationFunctionsCocomoIIStage3.Update(function);
        await _context.SaveChangesAsync();
        return function;
    }

    public async Task<bool> DeleteAsync(int functionId)
    {
        var function = await GetByIdAsync(functionId);
        if (function == null)
            return false;

        _context.EstimationFunctionsCocomoIIStage3.Remove(function);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BelongsToEstimationAsync(int functionId, int estimationId)
    {
        return await _context.EstimationFunctionsCocomoIIStage3
            .AnyAsync(f => f.FunctionId == functionId && f.EstimationId == estimationId);
    }

    public async Task<decimal> GetTotalUfpAsync(int estimationId)
    {
        var functions = await _context.EstimationFunctionsCocomoIIStage3
            .Where(f => f.EstimationId == estimationId)
            .ToListAsync();

        return functions.Sum(f => f.CalculatedPoints ?? 0);
    }
}