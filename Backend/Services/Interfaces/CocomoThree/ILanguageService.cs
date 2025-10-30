using Backend.Models.DTOs.CocomoThree;

namespace Backend.Services.Interfaces.CocomoThree;

/// <summary>
/// Service interface for Language operations
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Get all languages
    /// </summary>
    Task<IEnumerable<LanguageDto>> GetAllLanguagesAsync();

    /// <summary>
    /// Get language by ID
    /// </summary>
    Task<LanguageDto?> GetLanguageByIdAsync(int languageId);
}
