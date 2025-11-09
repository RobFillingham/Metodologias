using Backend.Data.Context;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.Implementations.CocomoIIStage3;

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
        return await _context.LanguagesCocomoIIStage3
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Language?> GetByIdAsync(int languageId)
    {
        return await _context.LanguagesCocomoIIStage3
            .FirstOrDefaultAsync(l => l.LanguageId == languageId);
    }

    public async Task<bool> ExistsAsync(int languageId)
    {
        return await _context.LanguagesCocomoIIStage3
            .AnyAsync(l => l.LanguageId == languageId);
    }

    public async Task<bool> LanguageNameExistsAsync(string name, int? excludeLanguageId = null)
    {
        var query = _context.LanguagesCocomoIIStage3.Where(l => l.Name.ToLower() == name.ToLower());

        if (excludeLanguageId.HasValue)
        {
            query = query.Where(l => l.LanguageId != excludeLanguageId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Language> CreateAsync(Language language)
    {
        _context.LanguagesCocomoIIStage3.Add(language);
        await _context.SaveChangesAsync();
        return language;
    }

    public async Task<Language> UpdateAsync(Language language)
    {
        _context.LanguagesCocomoIIStage3.Update(language);
        await _context.SaveChangesAsync();
        return language;
    }

    public async Task<bool> DeleteAsync(int languageId)
    {
        var language = await GetByIdAsync(languageId);

        if (language == null)
            return false;

        _context.LanguagesCocomoIIStage3.Remove(language);
        await _context.SaveChangesAsync();
        return true;
    }
}