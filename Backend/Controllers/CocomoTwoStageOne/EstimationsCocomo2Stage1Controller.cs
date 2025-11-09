using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoTwoStageOne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.CocomoTwoStageOne;

/// <summary>
/// Controller for managing Estimations (COCOMO 2 Stage 1)
/// </summary>
[ApiController]
[Route("api/cocomo2-stage1/estimations")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 1 - Estimations")]
public class EstimationsCocomo2Stage1Controller : ControllerBase
{
    private readonly IEstimationCocomo2Stage1Service _service;
    private readonly ILogger<EstimationsCocomo2Stage1Controller> _logger;

    public EstimationsCocomo2Stage1Controller(
        IEstimationCocomo2Stage1Service service,
        ILogger<EstimationsCocomo2Stage1Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get all estimations for a specific project
    /// </summary>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationCocomo2Stage1Dto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EstimationCocomo2Stage1Dto>>>> GetProjectEstimations(int projectId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var estimations = await _service.GetProjectEstimationsAsync(projectId, userId);

            var response = ApiResponse<IEnumerable<EstimationCocomo2Stage1Dto>>.SuccessResponse(
                estimations,
                "Estimations retrieved successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to project estimations: {ProjectId}", projectId);
            var errorResponse = ApiResponse<IEnumerable<EstimationCocomo2Stage1Dto>>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    /// <summary>
    /// Get a specific estimation by ID with all details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EstimationCocomo2Stage1Dto>>> GetEstimationById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var estimation = await _service.GetByIdAsync(id, userId);

            if (estimation == null)
            {
                var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                    "Estimation not found or access denied",
                    new List<string> { "The estimation does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<EstimationCocomo2Stage1Dto>.SuccessResponse(
                estimation,
                "Estimation retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new estimation for a project
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationCocomo2Stage1Dto>>> CreateEstimation([FromBody] CreateEstimationCocomo2Stage1Dto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _service.CreateAsync(createDto.ProjectId, createDto, userId);

            var response = ApiResponse<EstimationCocomo2Stage1Dto>.SuccessResponse(
                estimation,
                "Estimation created successfully"
            );

            return CreatedAtAction(
                nameof(GetEstimationById),
                new { id = estimation.EstimationId },
                response
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Referenced entity not found");
            var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                "Not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized estimation creation in project: {ProjectId}", createDto.ProjectId);
            var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating estimation for project {ProjectId}", createDto.ProjectId);
            throw;
        }
    }

    /// <summary>
    /// Update estimation ratings and recalculate
    /// </summary>
    [HttpPut("{id}/ratings")]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationCocomo2Stage1Dto>>> UpdateRatings(
        int id,
        [FromBody] UpdateRatingsCocomo2Stage1Dto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _service.UpdateRatingsAsync(id, updateDto, userId);

            var response = ApiResponse<EstimationCocomo2Stage1Dto>.SuccessResponse(
                estimation,
                "Ratings updated and estimation recalculated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on estimation: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ratings for estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update actual results (post-mortem comparison)
    /// </summary>
    [HttpPut("{id}/actual-results")]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EstimationCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationCocomo2Stage1Dto>>> UpdateActualResults(
        int id,
        [FromBody] UpdateActualResultsCocomo2Stage1Dto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _service.UpdateActualResultsAsync(id, updateDto, userId);

            var response = ApiResponse<EstimationCocomo2Stage1Dto>.SuccessResponse(
                estimation,
                "Actual results updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on estimation: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating actual results for estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete an estimation
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEstimation(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _service.DeleteAsync(id, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Estimation deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized delete attempt on estimation: {EstimationId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting estimation {EstimationId}", id);
            throw;
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        return userId;
    }
}
