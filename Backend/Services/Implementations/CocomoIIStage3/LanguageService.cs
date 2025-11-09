using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Services.Implementations.CocomoIIStage3;

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

    public async Task<LanguageDto> CreateLanguageAsync(CreateLanguageDto createLanguageDto)
    {
        // Validate that name doesn't already exist
        if (await _languageRepository.LanguageNameExistsAsync(createLanguageDto.Name))
        {
            throw new InvalidOperationException($"A language with the name '{createLanguageDto.Name}' already exists");
        }

        // Validate SLOC per UFP is positive
        if (createLanguageDto.SlocPerUfp <= 0)
        {
            throw new ArgumentException("SLOC per UFP must be greater than zero");
        }

        var language = new Language
        {
            Name = createLanguageDto.Name.Trim(),
            SlocPerUfp = createLanguageDto.SlocPerUfp
        };

        var createdLanguage = await _languageRepository.CreateAsync(language);

        return new LanguageDto
        {
            LanguageId = createdLanguage.LanguageId,
            Name = createdLanguage.Name,
            SlocPerUfp = createdLanguage.SlocPerUfp
        };
    }

    public async Task<LanguageDto?> UpdateLanguageAsync(int languageId, UpdateLanguageDto updateLanguageDto)
    {
        var existingLanguage = await _languageRepository.GetByIdAsync(languageId);

        if (existingLanguage == null)
            return null;

        // Validate that name doesn't already exist (excluding current language)
        if (await _languageRepository.LanguageNameExistsAsync(updateLanguageDto.Name, languageId))
        {
            throw new InvalidOperationException($"A language with the name '{updateLanguageDto.Name}' already exists");
        }

        // Validate SLOC per UFP is positive
        if (updateLanguageDto.SlocPerUfp <= 0)
        {
            throw new ArgumentException("SLOC per UFP must be greater than zero");
        }

        existingLanguage.Name = updateLanguageDto.Name.Trim();
        existingLanguage.SlocPerUfp = updateLanguageDto.SlocPerUfp;

        var updatedLanguage = await _languageRepository.UpdateAsync(existingLanguage);

        return new LanguageDto
        {
            LanguageId = updatedLanguage.LanguageId,
            Name = updatedLanguage.Name,
            SlocPerUfp = updatedLanguage.SlocPerUfp
        };
    }

    public async Task<bool> DeleteLanguageAsync(int languageId)
    {
        return await _languageRepository.DeleteAsync(languageId);
    }
}