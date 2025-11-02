using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs;
using Backend.Models.Responses;
using Backend.Services.Interfaces;

namespace Backend.Controllers;

/// <summary>
/// Controller for managing KLOC (Lines of Code) estimations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("KLOC Estimations")]
public class KlocEstimationsController : ControllerBase
{
    private readonly IKlocEstimationService _klocEstimationService;
    private readonly ILogger<KlocEstimationsController> _logger;

    public KlocEstimationsController(
        IKlocEstimationService klocEstimationService,
        ILogger<KlocEstimationsController> logger)
    {
        _klocEstimationService = klocEstimationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all KLOC estimations for a specific project
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>List of KLOC estimations for the project</returns>
    /// <response code="200">Returns the list of KLOC estimations</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<KlocEstimationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<KlocEstimationDto>>>> GetEstimationsByProject(int projectId)
    {
        try
        {
            var estimations = await _klocEstimationService.GetEstimationsByProjectAsync(projectId);

            var response = ApiResponse<IEnumerable<KlocEstimationDto>>.SuccessResponse(
                estimations,
                "KLOC Estimations retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving KLOC estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    /// <summary>
    /// Get a specific KLOC estimation by ID
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <returns>The KLOC estimation details</returns>
    /// <response code="200">Returns the KLOC estimation</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<KlocEstimationDto>>> GetEstimationById(int id)
    {
        try
        {
            var estimation = await _klocEstimationService.GetEstimationByIdAsync(id);

            if (estimation == null)
            {
                var errorResponse = ApiResponse<KlocEstimationDto>.ErrorResponse(
                    "KLOC estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<KlocEstimationDto>.SuccessResponse(
                estimation,
                "KLOC estimation retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving KLOC estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new KLOC estimation
    /// </summary>
    /// <param name="createDto">The KLOC estimation creation data</param>
    /// <returns>The created KLOC estimation with calculated metrics</returns>
    /// <response code="201">KLOC estimation created successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<KlocEstimationDto>>> CreateEstimation([FromBody] CreateKlocEstimationDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<KlocEstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var estimation = await _klocEstimationService.CreateEstimationAsync(createDto);

            var response = ApiResponse<KlocEstimationDto>.SuccessResponse(
                estimation,
                "KLOC estimation created successfully"
            );

            return CreatedAtAction(nameof(GetEstimationById), new { id = estimation.KlocEstimationId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating KLOC estimation");
            throw;
        }
    }

    /// <summary>
    /// Update an existing KLOC estimation
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <param name="updateDto">The KLOC estimation update data</param>
    /// <returns>The updated KLOC estimation with recalculated metrics</returns>
    /// <response code="200">KLOC estimation updated successfully</response>
    /// <response code="400">If input validation fails or ID mismatch</response>
    /// <response code="404">If estimation not found</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<KlocEstimationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<KlocEstimationDto>>> UpdateEstimation(int id, [FromBody] UpdateKlocEstimationDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<KlocEstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            if (id != updateDto.KlocEstimationId)
            {
                var errorResponse = ApiResponse<KlocEstimationDto>.ErrorResponse(
                    "ID mismatch",
                    new List<string> { "The estimation ID in the URL does not match the ID in the request body" }
                );
                return BadRequest(errorResponse);
            }

            var estimation = await _klocEstimationService.UpdateEstimationAsync(updateDto);

            var response = ApiResponse<KlocEstimationDto>.SuccessResponse(
                estimation,
                "KLOC estimation updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "KLOC estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<KlocEstimationDto>.ErrorResponse(
                "KLOC estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating KLOC estimation {EstimationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a KLOC estimation
    /// </summary>
    /// <param name="id">The estimation ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">KLOC estimation deleted successfully</response>
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
            var result = await _klocEstimationService.DeleteEstimationAsync(id);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "KLOC estimation not found",
                    new List<string> { "The estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "KLOC estimation deleted successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting KLOC estimation {EstimationId}", id);
            throw;
        }
    }
}
