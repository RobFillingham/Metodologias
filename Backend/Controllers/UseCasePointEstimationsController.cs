using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs;
using Backend.Models.Responses;
using Backend.Services.Interfaces;

namespace Backend.Controllers;

/// <summary>
/// Controller for managing Use Case Point estimations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Use Case Point Estimations")]
public class UseCasePointEstimationsController : ControllerBase
{
    private readonly IUseCasePointEstimationService _ucpEstimationService;
    private readonly ILogger<UseCasePointEstimationsController> _logger;

    public UseCasePointEstimationsController(
        IUseCasePointEstimationService ucpEstimationService,
        ILogger<UseCasePointEstimationsController> logger)
    {
        _ucpEstimationService = ucpEstimationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all Use Case Point estimations for a specific project
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>List of Use Case Point estimations for the project</returns>
    /// <response code="200">Returns the list of Use Case Point estimations</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UseCasePointEstimationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UseCasePointEstimationDto>>>> GetEstimationsByProject(int projectId)
    {
        try
        {
            var estimations = await _ucpEstimationService.GetEstimationsByProjectAsync(projectId);

            var response = ApiResponse<IEnumerable<UseCasePointEstimationDto>>.SuccessResponse(
                estimations,
                "Use Case Point Estimations retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Use Case Point estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    /// <summary>
    /// Get a specific Use Case Point estimation by ID
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <returns>The Use Case Point estimation details</returns>
    /// <response code="200">Returns the Use Case Point estimation</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UseCasePointEstimationDto>>> GetEstimationById(int id)
    {
        try
        {
            var estimation = await _ucpEstimationService.GetEstimationByIdAsync(id);

            if (estimation == null)
            {
                var errorResponse = ApiResponse<UseCasePointEstimationDto>.ErrorResponse(
                    "Use Case Point estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<UseCasePointEstimationDto>.SuccessResponse(
                estimation,
                "Use Case Point estimation retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Use Case Point estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new Use Case Point estimation
    /// </summary>
    /// <param name="createDto">The Use Case Point estimation creation data</param>
    /// <returns>The created Use Case Point estimation with calculated metrics</returns>
    /// <response code="201">Use Case Point estimation created successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UseCasePointEstimationDto>>> CreateEstimation([FromBody] CreateUseCasePointEstimationDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<UseCasePointEstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var estimation = await _ucpEstimationService.CreateEstimationAsync(createDto);

            var response = ApiResponse<UseCasePointEstimationDto>.SuccessResponse(
                estimation,
                "Use Case Point estimation created successfully"
            );

            return CreatedAtAction(nameof(GetEstimationById), new { id = estimation.UcpEstimationId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Use Case Point estimation");
            throw;
        }
    }

    /// <summary>
    /// Update an existing Use Case Point estimation
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <param name="updateDto">The Use Case Point estimation update data</param>
    /// <returns>The updated Use Case Point estimation with recalculated metrics</returns>
    /// <response code="200">Use Case Point estimation updated successfully</response>
    /// <response code="400">If input validation fails or ID mismatch</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UseCasePointEstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UseCasePointEstimationDto>>> UpdateEstimation(int id, [FromBody] UpdateUseCasePointEstimationDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<UseCasePointEstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            if (id != updateDto.UcpEstimationId)
            {
                var errorResponse = ApiResponse<UseCasePointEstimationDto>.ErrorResponse(
                    "ID mismatch",
                    new List<string> { "The estimation ID in the URL does not match the ID in the request body" }
                );
                return BadRequest(errorResponse);
            }

            var estimation = await _ucpEstimationService.UpdateEstimationAsync(updateDto);

            var response = ApiResponse<UseCasePointEstimationDto>.SuccessResponse(
                estimation,
                "Use Case Point estimation updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Use Case Point estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<UseCasePointEstimationDto>.ErrorResponse(
                "Use Case Point estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Use Case Point estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a Use Case Point estimation
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Use Case Point estimation deleted successfully</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteEstimation(int id)
    {
        try
        {
            var result = await _ucpEstimationService.DeleteEstimationAsync(id);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Use Case Point estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Use Case Point estimation deleted successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Use Case Point estimation {EstimationId}", id);
            throw;
        }
    }
}
