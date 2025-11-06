using Backend.Models.Entities.CocomoThree;

namespace Backend.Repositories.Interfaces.CocomoThree;

/// <summary>
/// Repository interface for Language entity operations
/// </summary>
public interface ILanguageRepository
{
    /// <summary>
    /// Get all languages
    /// </summary>
    Task<IEnumerable<Language>> GetAllAsync();

    /// <summary>
    /// Get language by ID
    /// </summary>
    Task<Language?> GetByIdAsync(int languageId);

    /// <summary>
    /// Check if language exists
    /// </summary>
    Task<bool> ExistsAsync(int languageId);

    /// <summary>
    /// Check if a language with the same name exists (case-insensitive)
    /// </summary>
    Task<bool> LanguageNameExistsAsync(string name, int? excludeLanguageId = null);

    /// <summary>
    /// Create a new language
    /// </summary>
    Task<Language> CreateAsync(Language language);

    /// <summary>
    /// Update an existing language
    /// </summary>
    Task<Language> UpdateAsync(Language language);

    /// <summary>
    /// Delete a language
    /// </summary>
    Task<bool> DeleteAsync(int languageId);
}
