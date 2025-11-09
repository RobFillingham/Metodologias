using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Controllers.CocomoIIStage3;

/// <summary>
/// Controller for managing COCOMO II Stage 3 estimations
/// </summary>
[ApiController]
[Route("api/Projects/{projectId}/CocomoIIStage3/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 3 - Estimations")]
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
            _logger.LogWarning(ex, "Unauthorized access attempt for project {ProjectId}", projectId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving estimations for project {ProjectId}", projectId);
            var response = ApiResponse<IEnumerable<EstimationDto>>.ErrorResponse(
                "An error occurred while retrieving estimations"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Get a specific estimation by ID
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="estimationId">The estimation ID</param>
    /// <returns>The estimation details</returns>
    /// <response code="200">Returns the estimation</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    /// <response code="404">If estimation not found</response>
    [HttpGet("{estimationId}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> GetEstimation(int projectId, int estimationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var estimation = await _estimationService.GetEstimationByIdAsync(estimationId, userId);

            if (estimation == null)
            {
                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse("Estimation not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<EstimationDto>.SuccessResponse(
                estimation,
                "Estimation retrieved successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for estimation {EstimationId}", estimationId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving estimation {EstimationId}", estimationId);
            var response = ApiResponse<EstimationDto>.ErrorResponse(
                "An error occurred while retrieving the estimation"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Create a new estimation
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="createDto">The estimation creation data</param>
    /// <returns>The created estimation</returns>
    /// <response code="201">Returns the created estimation</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> CreateEstimation(int projectId, CreateEstimationDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _estimationService.CreateEstimationAsync(projectId, createDto, userId);

            var response = ApiResponse<EstimationDto>.SuccessResponse(
                estimation,
                "Estimation created successfully"
            );

            return CreatedAtAction(
                nameof(GetEstimation),
                new { projectId, estimationId = estimation.EstimationId },
                response
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for project {ProjectId}", projectId);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for estimation creation in project {ProjectId}", projectId);
            var response = ApiResponse<EstimationDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating estimation for project {ProjectId}", projectId);
            var response = ApiResponse<EstimationDto>.ErrorResponse(
                "An error occurred while creating the estimation"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Update estimation ratings (SF and EM values)
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="updateDto">The rating update data</param>
    /// <returns>The updated estimation</returns>
    /// <response code="200">Returns the updated estimation</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    /// <response code="404">If estimation not found</response>
    [HttpPut("{estimationId}/ratings")]
    [ProducesResponseType(typeof(ApiResponse<EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EstimationDto>>> UpdateEstimationRatings(int projectId, int estimationId, UpdateEstimationRatingsDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<EstimationDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var estimation = await _estimationService.UpdateEstimationRatingsAsync(estimationId, updateDto, userId);

            var response = ApiResponse<EstimationDto>.SuccessResponse(
                estimation,
                "Estimation ratings updated successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for estimation {EstimationId}", estimationId);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation {EstimationId} not found", estimationId);
            var response = ApiResponse<EstimationDto>.ErrorResponse("Estimation not found");
            return NotFound(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid rating data for estimation {EstimationId}", estimationId);
            var response = ApiResponse<EstimationDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ratings for estimation {EstimationId}", estimationId);
            var response = ApiResponse<EstimationDto>.ErrorResponse(
                "An error occurred while updating estimation ratings"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Delete an estimation
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="estimationId">The estimation ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Estimation deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    /// <response code="404">If estimation not found</response>
    [HttpDelete("{estimationId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEstimation(int projectId, int estimationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _estimationService.DeleteEstimationAsync(estimationId, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Estimation not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Estimation deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for estimation {EstimationId}", estimationId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting estimation {EstimationId}", estimationId);
            var response = ApiResponse<object>.ErrorResponse(
                "An error occurred while deleting the estimation"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity");
        }
        return userId;
    }
}