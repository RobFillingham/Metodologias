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

    public async Task<bool> LanguageNameExistsAsync(string name, int? excludeLanguageId = null)
    {
        var query = _context.Languages.Where(l => l.Name.ToLower() == name.ToLower());
        
        if (excludeLanguageId.HasValue)
        {
            query = query.Where(l => l.LanguageId != excludeLanguageId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Language> CreateAsync(Language language)
    {
        _context.Languages.Add(language);
        await _context.SaveChangesAsync();
        return language;
    }

    public async Task<Language> UpdateAsync(Language language)
    {
        _context.Languages.Update(language);
        await _context.SaveChangesAsync();
        return language;
    }

    public async Task<bool> DeleteAsync(int languageId)
    {
        var language = await GetByIdAsync(languageId);
        
        if (language == null)
            return false;

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync();
        return true;
    }
}
