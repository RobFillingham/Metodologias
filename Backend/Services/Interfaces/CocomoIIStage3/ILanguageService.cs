using Backend.Models.DTOs.CocomoIIStage3;

namespace Backend.Services.Interfaces.CocomoIIStage3;

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

    /// <summary>
    /// Create a new language
    /// </summary>
    Task<LanguageDto> CreateLanguageAsync(CreateLanguageDto createLanguageDto);

    /// <summary>
    /// Update an existing language
    /// </summary>
    Task<LanguageDto?> UpdateLanguageAsync(int languageId, UpdateLanguageDto updateLanguageDto);

    /// <summary>
    /// Delete a language
    /// </summary>
    Task<bool> DeleteLanguageAsync(int languageId);
}