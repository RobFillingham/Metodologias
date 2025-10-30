using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Controllers.CocomoThree;

/// <summary>
/// Controller for managing programming languages
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LanguagesController : ControllerBase
{
    private readonly ILanguageService _languageService;
    private readonly ILogger<LanguagesController> _logger;

    public LanguagesController(
        ILanguageService languageService,
        ILogger<LanguagesController> logger)
    {
        _languageService = languageService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available programming languages
    /// </summary>
    /// <returns>List of languages with SLOC conversion factors</returns>
    /// <response code="200">Returns the list of languages</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LanguageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<LanguageDto>>>> GetAllLanguages()
    {
        try
        {
            var languages = await _languageService.GetAllLanguagesAsync();

            var response = ApiResponse<IEnumerable<LanguageDto>>.SuccessResponse(
                languages,
                "Languages retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving languages");
            throw;
        }
    }

    /// <summary>
    /// Get a specific language by ID
    /// </summary>
    /// <param name="id">The language ID</param>
    /// <returns>The language details</returns>
    /// <response code="200">Returns the language</response>
    /// <response code="404">If language not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> GetLanguageById(int id)
    {
        try
        {
            var language = await _languageService.GetLanguageByIdAsync(id);

            if (language == null)
            {
                var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                    "Language not found",
                    new List<string> { $"Language with ID {id} does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<LanguageDto>.SuccessResponse(
                language,
                "Language retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving language {LanguageId}", id);
            throw;
        }
    }
}
