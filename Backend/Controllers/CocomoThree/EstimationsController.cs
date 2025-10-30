using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Controllers.CocomoThree;

/// <summary>
/// Controller for managing COCOMO estimations
/// </summary>
[ApiController]
[Route("api/Projects/{projectId}/[controller]")]
[Authorize]
[Produces("application/json")]
public class EstimationsController : ControllerBase
{
    private readonly IEstimationService _estimationService;
    private readonly ILogger<EstimationsController> _logger;

    public EstimationsController(
        IEstimationService estimationService,
        ILogger<EstimationsController> logger)
    {
        _estimationService = estimationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all estimations for a project
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>List of estimations</returns>
    /// <response code="200">Returns the list of estimations</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EstimationDto>>>> GetProjectEstimations(int projectId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var estimations = await _estimationService.GetProjectEstimationsAsync(projectId, userId);

            var response = ApiResponse<IEnumerable<EstimationDto>>.SuccessResponse(
                estimations,
                "Estimations retrieved successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to project estimations: {ProjectId}", projectId);
            var errorResponse = ApiResponse<IEnumerable<EstimationDto>>.ErrorResponse(
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
    /// <param name="projectId">The project ID</param>
    /// <param name="id">The estimation ID</param>
    /// <returns>The estimation with all details (functions, results, etc.)</returns>
    /// <response code="200">Returns the estimation</response>
    /// <response code="404">If estimation not found or not accessible</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> GetEstimationById(int projectId, int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var estimation = await _estimationService.GetEstimationByIdAsync(id, userId);

            if (estimation == null)
            {
                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                    "Estimation not found or access denied",
                    new List<string> { "The estimation does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<EstimationDto>.SuccessResponse(
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
    /// <param name="projectId">The project ID</param>
    /// <param name="createDto">The estimation creation data</param>
    /// <returns>The created estimation</returns>
    /// <response code="201">Estimation created successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> CreateEstimation(int projectId, [FromBody] CreateEstimationDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _estimationService.CreateEstimationAsync(projectId, createDto, userId);

            var response = ApiResponse<EstimationDto>.SuccessResponse(
                estimation,
                "Estimation created successfully"
            );

            return CreatedAtAction(
                nameof(GetEstimationById), 
                new { projectId = projectId, id = estimation.EstimationId }, 
                response
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Referenced entity not found");
            var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                "Not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized estimation creation in project: {ProjectId}", projectId);
            var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating estimation for project {ProjectId}", projectId);
            throw;
        }
    }

    /// <summary>
    /// Update estimation ratings (SF and EM) and recalculate
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="id">The estimation ID</param>
    /// <param name="updateDto">The ratings update data</param>
    /// <returns>The updated estimation with recalculated results</returns>
    /// <response code="200">Estimation updated and recalculated successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpPut("{id}/Ratings")]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> UpdateEstimationRatings(
        int projectId, 
        int id, 
        [FromBody] UpdateEstimationRatingsDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _estimationService.UpdateEstimationRatingsAsync(id, updateDto, userId);

            var response = ApiResponse<EstimationDto>.SuccessResponse(
                estimation,
                "Estimation ratings updated and recalculated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on estimation: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating estimation ratings {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update actual results (post-mortem comparison)
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="id">The estimation ID</param>
    /// <param name="updateDto">The actual results data</param>
    /// <returns>The updated estimation</returns>
    /// <response code="200">Actual results updated successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpPut("{id}/ActualResults")]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> UpdateActualResults(
        int projectId, 
        int id, 
        [FromBody] UpdateActualResultsDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _estimationService.UpdateActualResultsAsync(id, updateDto, userId);

            var response = ApiResponse<EstimationDto>.SuccessResponse(
                estimation,
                "Actual results updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on estimation: {EstimationId}", id);
            var errorResponse = ApiResponse<EstimationDto>.ErrorResponse(
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
    /// <param name="projectId">The project ID</param>
    /// <param name="id">The estimation ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Estimation deleted successfully</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEstimation(int projectId, int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _estimationService.DeleteEstimationAsync(id, userId);

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

    /// <summary>
    /// Extract the current user ID from the JWT token
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        
        if (userIdClaim == null)
        {
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            _logger.LogWarning("User ID claim not found. Available claims: {Claims}", string.Join(", ", claims));
            throw new UnauthorizedAccessException("Invalid user token");
        }
        
        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("User ID claim value is not a valid integer: {Value}", userIdClaim.Value);
            throw new UnauthorizedAccessException("Invalid user token");
        }
        
        return userId;
    }
}
