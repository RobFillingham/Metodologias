using Microsoft.AspNetCore.Authorization;
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
[Tags("COCOMO II - Languages")]
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

    /// <summary>
    /// Create a new programming language
    /// </summary>
    /// <param name="createLanguageDto">The language data to create</param>
    /// <returns>The created language</returns>
    /// <response code="201">Returns the newly created language</response>
    /// <response code="400">If the language data is invalid</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> CreateLanguage([FromBody] CreateLanguageDto createLanguageDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );

                return BadRequest(errorResponse);
            }

            var language = await _languageService.CreateLanguageAsync(createLanguageDto);

            var response = ApiResponse<LanguageDto>.SuccessResponse(
                language,
                "Language created successfully"
            );

            return CreatedAtAction(nameof(GetLanguageById), new { id = language.LanguageId }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create language: {Message}", ex.Message);
            var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                "Failed to create language",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid language data: {Message}", ex.Message);
            var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                "Invalid language data",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating language");
            throw;
        }
    }

    /// <summary>
    /// Update an existing programming language
    /// </summary>
    /// <param name="id">The language ID</param>
    /// <param name="updateLanguageDto">The updated language data</param>
    /// <returns>The updated language</returns>
    /// <response code="200">Returns the updated language</response>
    /// <response code="400">If the language data is invalid</response>
    /// <response code="404">If language not found</response>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> UpdateLanguage(int id, [FromBody] UpdateLanguageDto updateLanguageDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );

                return BadRequest(errorResponse);
            }

            var language = await _languageService.UpdateLanguageAsync(id, updateLanguageDto);

            if (language == null)
            {
                var notFoundResponse = ApiResponse<LanguageDto>.ErrorResponse(
                    "Language not found",
                    new List<string> { $"Language with ID {id} does not exist" }
                );
                return NotFound(notFoundResponse);
            }

            var response = ApiResponse<LanguageDto>.SuccessResponse(
                language,
                "Language updated successfully"
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update language {LanguageId}: {Message}", id, ex.Message);
            var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                "Failed to update language",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid language data for {LanguageId}: {Message}", id, ex.Message);
            var errorResponse = ApiResponse<LanguageDto>.ErrorResponse(
                "Invalid language data",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating language {LanguageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a programming language
    /// </summary>
    /// <param name="id">The language ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">If language was deleted successfully</response>
    /// <response code="404">If language not found</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLanguage(int id)
    {
        try
        {
            var deleted = await _languageService.DeleteLanguageAsync(id);

            if (!deleted)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Language not found",
                    new List<string> { $"Language with ID {id} does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object?>.SuccessResponse(
                null,
                "Language deleted successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting language {LanguageId}", id);
            throw;
        }
    }
}
