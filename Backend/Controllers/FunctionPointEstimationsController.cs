using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs;
using Backend.Models.Responses;
using Backend.Services.Interfaces;

namespace Backend.Controllers;

/// <summary>
/// Controller for managing Function Point estimations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Function Point Estimations")]
public class FunctionPointEstimationsController : ControllerBase
{
    private readonly IFunctionPointEstimationService _fpEstimationService;
    private readonly ILogger<FunctionPointEstimationsController> _logger;

    public FunctionPointEstimationsController(
        IFunctionPointEstimationService fpEstimationService,
        ILogger<FunctionPointEstimationsController> logger)
    {
        _fpEstimationService = fpEstimationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all Function Point estimations for a specific project
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>List of Function Point estimations for the project</returns>
    /// <response code="200">Returns the list of Function Point estimations</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FunctionPointEstimationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FunctionPointEstimationDto>>>> GetEstimationsByProject(int projectId)
    {
        try
        {
            var estimations = await _fpEstimationService.GetEstimationsByProjectAsync(projectId);

            var response = ApiResponse<IEnumerable<FunctionPointEstimationDto>>.SuccessResponse(
                estimations,
                "Function Point Estimations retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Function Point estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    /// <summary>
    /// Get a specific Function Point estimation by ID
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <returns>The Function Point estimation details</returns>
    /// <response code="200">Returns the Function Point estimation</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FunctionPointEstimationDto>>> GetEstimationById(int id)
    {
        try
        {
            var estimation = await _fpEstimationService.GetEstimationByIdAsync(id);

            if (estimation == null)
            {
                var errorResponse = ApiResponse<FunctionPointEstimationDto>.ErrorResponse(
                    "Function Point estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<FunctionPointEstimationDto>.SuccessResponse(
                estimation,
                "Function Point estimation retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Function Point estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new Function Point estimation
    /// </summary>
    /// <param name="createDto">The Function Point estimation creation data</param>
    /// <returns>The created Function Point estimation with calculated metrics</returns>
    /// <response code="201">Function Point estimation created successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FunctionPointEstimationDto>>> CreateEstimation([FromBody] CreateFunctionPointEstimationDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<FunctionPointEstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var estimation = await _fpEstimationService.CreateEstimationAsync(createDto);

            var response = ApiResponse<FunctionPointEstimationDto>.SuccessResponse(
                estimation,
                "Function Point estimation created successfully"
            );

            return CreatedAtAction(nameof(GetEstimationById), new { id = estimation.FpEstimationId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Function Point estimation");
            throw;
        }
    }

    /// <summary>
    /// Update an existing Function Point estimation
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <param name="updateDto">The Function Point estimation update data</param>
    /// <returns>The updated Function Point estimation with recalculated metrics</returns>
    /// <response code="200">Function Point estimation updated successfully</response>
    /// <response code="400">If input validation fails or ID mismatch</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<FunctionPointEstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FunctionPointEstimationDto>>> UpdateEstimation(int id, [FromBody] UpdateFunctionPointEstimationDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<FunctionPointEstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            if (id != updateDto.FpEstimationId)
            {
                var errorResponse = ApiResponse<FunctionPointEstimationDto>.ErrorResponse(
                    "ID mismatch",
                    new List<string> { "The estimation ID in the URL does not match the ID in the request body" }
                );
                return BadRequest(errorResponse);
            }

            var estimation = await _fpEstimationService.UpdateEstimationAsync(updateDto);

            var response = ApiResponse<FunctionPointEstimationDto>.SuccessResponse(
                estimation,
                "Function Point estimation updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Function Point estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<FunctionPointEstimationDto>.ErrorResponse(
                "Function Point estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Function Point estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a Function Point estimation
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Function Point estimation deleted successfully</response>
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
            var result = await _fpEstimationService.DeleteEstimationAsync(id);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Function Point estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Function Point estimation deleted successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Function Point estimation {EstimationId}", id);
            throw;
        }
    }
}
