using Backend.Models.DTOs.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Services.Implementations.CocomoThree;

/// <summary>
/// Service implementation for Language operations
/// </summary>
public class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _languageRepository;

    public LanguageService(ILanguageRepository languageRepository)
    {
        _languageRepository = languageRepository;
    }

    public async Task<IEnumerable<LanguageDto>> GetAllLanguagesAsync()
    {
        var languages = await _languageRepository.GetAllAsync();
        
        return languages.Select(l => new LanguageDto
        {
            LanguageId = l.LanguageId,
            Name = l.Name,
            SlocPerUfp = l.SlocPerUfp
        });
    }

    public async Task<LanguageDto?> GetLanguageByIdAsync(int languageId)
    {
        var language = await _languageRepository.GetByIdAsync(languageId);
        
        if (language == null)
            return null;

        return new LanguageDto
        {
            LanguageId = language.LanguageId,
            Name = language.Name,
            SlocPerUfp = language.SlocPerUfp
        };
    }
}
