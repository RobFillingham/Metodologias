using Backend.Data.Context;
using Backend.Models.Entities.CocomoOne;
using Backend.Repositories.Interfaces.CocomoOne;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories.Implementations.CocomoOne;

public class Cocomo1EstimationRepository : ICocomo1EstimationRepository
{
    private readonly ApplicationDbContext _context;

    public Cocomo1EstimationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cocomo1Estimation>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Cocomo1Estimations
            .Where(e => e.ProjectId == projectId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Cocomo1Estimation?> GetByIdAsync(int cocomo1EstimationId)
    {
        return await _context.Cocomo1Estimations
            .FirstOrDefaultAsync(e => e.Cocomo1EstimationId == cocomo1EstimationId);
    }

    public async Task<Cocomo1Estimation> CreateAsync(Cocomo1Estimation estimation)
    {
        _context.Cocomo1Estimations.Add(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<Cocomo1Estimation> UpdateAsync(Cocomo1Estimation estimation)
    {
        estimation.UpdatedAt = System.DateTime.UtcNow;
        _context.Cocomo1Estimations.Update(estimation);
        await _context.SaveChangesAsync();
        return estimation;
    }

    public async Task<bool> DeleteAsync(int cocomo1EstimationId)
    {
        var estimation = await GetByIdAsync(cocomo1EstimationId);
        if (estimation == null)
            return false;

        _context.Cocomo1Estimations.Remove(estimation);
        await _context.SaveChangesAsync();
        return true;
    }
}
