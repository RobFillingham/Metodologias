using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Controllers.CocomoIIStage3;

/// <summary>
/// Controller for managing programming languages
/// </summary>
[ApiController]
[Route("api/CocomoIIStage3/[controller]")]
[Produces("application/json")]
[Tags("COCOMO II Stage 3 - Languages")]
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
            var response = ApiResponse<IEnumerable<LanguageDto>>.ErrorResponse(
                "An error occurred while retrieving languages"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Get a specific language by ID
    /// </summary>
    /// <param name="languageId">The language ID</param>
    /// <returns>The language details</returns>
    /// <response code="200">Returns the language</response>
    /// <response code="404">If language not found</response>
    [HttpGet("{languageId}")]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> GetLanguage(int languageId)
    {
        try
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            if (language == null)
            {
                var errorResponse = ApiResponse<LanguageDto>.ErrorResponse("Language not found");
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
            _logger.LogError(ex, "Error retrieving language {LanguageId}", languageId);
            var response = ApiResponse<LanguageDto>.ErrorResponse(
                "An error occurred while retrieving the language"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Create a new programming language
    /// </summary>
    /// <param name="createDto">The language creation data</param>
    /// <returns>The created language</returns>
    /// <response code="201">Returns the created language</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not authorized</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> CreateLanguage(CreateLanguageDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<LanguageDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var language = await _languageService.CreateLanguageAsync(createDto);

            var response = ApiResponse<LanguageDto>.SuccessResponse(
                language,
                "Language created successfully"
            );

            return CreatedAtAction(
                nameof(GetLanguage),
                new { languageId = language.LanguageId },
                response
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to create language");
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for language creation");
            var response = ApiResponse<LanguageDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating language");
            var response = ApiResponse<LanguageDto>.ErrorResponse(
                "An error occurred while creating the language"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Update an existing programming language
    /// </summary>
    /// <param name="languageId">The language ID</param>
    /// <param name="updateDto">The language update data</param>
    /// <returns>The updated language</returns>
    /// <response code="200">Returns the updated language</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not authorized</response>
    /// <response code="404">If language not found</response>
    [HttpPut("{languageId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<LanguageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> UpdateLanguage(int languageId, UpdateLanguageDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<LanguageDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var language = await _languageService.UpdateLanguageAsync(languageId, updateDto);

            var response = ApiResponse<LanguageDto>.SuccessResponse(
                language,
                "Language updated successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to update language {LanguageId}", languageId);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Language {LanguageId} not found", languageId);
            var response = ApiResponse<LanguageDto>.ErrorResponse("Language not found");
            return NotFound(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for language {LanguageId} update", languageId);
            var response = ApiResponse<LanguageDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating language {LanguageId}", languageId);
            var response = ApiResponse<LanguageDto>.ErrorResponse(
                "An error occurred while updating the language"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Delete a programming language
    /// </summary>
    /// <param name="languageId">The language ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Language deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user is not authorized</response>
    /// <response code="404">If language not found</response>
    [HttpDelete("{languageId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLanguage(int languageId)
    {
        try
        {
            var result = await _languageService.DeleteLanguageAsync(languageId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Language not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Language deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to delete language {LanguageId}", languageId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting language {LanguageId}", languageId);
            var response = ApiResponse<object>.ErrorResponse(
                "An error occurred while deleting the language"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}