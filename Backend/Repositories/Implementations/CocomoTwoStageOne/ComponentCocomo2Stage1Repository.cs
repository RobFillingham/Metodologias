using Backend.Data.Context;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoTwoStageOne;

/// <summary>
/// Repository implementation for Component (COCOMO 2 Stage 1)
/// </summary>
public class ComponentCocomo2Stage1Repository : IComponentCocomo2Stage1Repository
{
    private readonly ApplicationDbContext _context;

    public ComponentCocomo2Stage1Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EstimationComponentCocomo2Stage1?> GetByIdAsync(int componentId)
    {
        return await _context.EstimationComponentsCocomo2Stage1
            .FindAsync(componentId);
    }

    public async Task<IEnumerable<EstimationComponentCocomo2Stage1>> GetByEstimationIdAsync(int estimationId)
    {
        return await _context.EstimationComponentsCocomo2Stage1
            .Where(c => c.EstimationId == estimationId)
            .OrderBy(c => c.ComponentName)
            .ToListAsync();
    }

    public async Task<EstimationComponentCocomo2Stage1> CreateAsync(EstimationComponentCocomo2Stage1 component)
    {
        component.CreatedAt = DateTime.UtcNow;
        component.UpdatedAt = DateTime.UtcNow;
        _context.EstimationComponentsCocomo2Stage1.Add(component);
        await _context.SaveChangesAsync();
        return component;
    }

    public async Task<IEnumerable<EstimationComponentCocomo2Stage1>> CreateBatchAsync(IEnumerable<EstimationComponentCocomo2Stage1> components)
    {
        var componentList = components.ToList();
        foreach (var component in componentList)
        {
            component.CreatedAt = DateTime.UtcNow;
            component.UpdatedAt = DateTime.UtcNow;
        }

        _context.EstimationComponentsCocomo2Stage1.AddRange(componentList);
        await _context.SaveChangesAsync();
        return componentList;
    }

    public async Task<EstimationComponentCocomo2Stage1> UpdateAsync(EstimationComponentCocomo2Stage1 component)
    {
        component.UpdatedAt = DateTime.UtcNow;
        _context.EstimationComponentsCocomo2Stage1.Update(component);
        await _context.SaveChangesAsync();
        return component;
    }

    public async Task<bool> DeleteAsync(int componentId)
    {
        var component = await _context.EstimationComponentsCocomo2Stage1
            .FindAsync(componentId);

        if (component == null)
            return false;

        _context.EstimationComponentsCocomo2Stage1.Remove(component);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalFpAsync(int estimationId)
    {
        var components = await _context.EstimationComponentsCocomo2Stage1
            .Where(c => c.EstimationId == estimationId)
            .ToListAsync();

        // Calculate effective FP considering reuse percentage
        decimal totalFp = 0;
        foreach (var component in components)
        {
            decimal effectiveFp = component.SizeFp;

            // If it's a reused component, reduce FP by reuse percentage
            if (component.ReusePercent.HasValue && component.ReusePercent.Value > 0)
            {
                var reuseReduction = component.ReusePercent.Value / 100.0m;
                effectiveFp = component.SizeFp * (1 - reuseReduction);
            }

            // If there are changes, add effort for those changes
            if (component.ChangePercent.HasValue && component.ChangePercent.Value > 0)
            {
                var changeAddition = (component.SizeFp * component.ChangePercent.Value / 100.0m);
                effectiveFp += changeAddition;
            }

            totalFp += effectiveFp;
        }

        return totalFp;
    }
}
