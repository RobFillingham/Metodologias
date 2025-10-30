using Backend.Data.Context;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoThree;

/// <summary>
/// Repository implementation for Language entity operations
/// </summary>
public class LanguageRepository : ILanguageRepository
{
    private readonly ApplicationDbContext _context;

    public LanguageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Language>> GetAllAsync()
    {
        return await _context.Languages
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Language?> GetByIdAsync(int languageId)
    {
        return await _context.Languages
            .FirstOrDefaultAsync(l => l.LanguageId == languageId);
    }

    public async Task<bool> ExistsAsync(int languageId)
    {
        return await _context.Languages
            .AnyAsync(l => l.LanguageId == languageId);
    }
}
