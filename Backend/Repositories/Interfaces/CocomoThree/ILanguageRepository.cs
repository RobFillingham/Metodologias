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
}
