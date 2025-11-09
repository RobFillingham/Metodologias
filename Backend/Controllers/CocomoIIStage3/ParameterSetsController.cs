using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Controllers.CocomoIIStage3;

/// <summary>
/// Controller for managing COCOMO II Stage 3 Parameter Sets
/// </summary>
[ApiController]
[Route("api/CocomoIIStage3/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 3 - Parameter Sets")]
public class ParameterSetsController : ControllerBase
{
    private readonly IParameterSetService _parameterSetService;
    private readonly ILogger<ParameterSetsController> _logger;

    public ParameterSetsController(
        IParameterSetService parameterSetService,
        ILogger<ParameterSetsController> logger)
    {
        _parameterSetService = parameterSetService;
        _logger = logger;
    }

    /// <summary>
    /// Get all parameter sets for the current user
    /// </summary>
    /// <returns>List of user's parameter sets</returns>
    /// <response code="200">Returns the list of parameter sets</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ParameterSetDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParameterSetDto>>>> GetMyParameterSets()
    {
        try
        {
            var userId = GetCurrentUserId();
            var parameterSets = await _parameterSetService.GetUserParameterSetsAsync(userId);

            var response = ApiResponse<IEnumerable<ParameterSetDto>>.SuccessResponse(
                parameterSets,
                "Parameter sets retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter sets for user");
            var response = ApiResponse<IEnumerable<ParameterSetDto>>.ErrorResponse(
                "An error occurred while retrieving parameter sets"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Get default parameter sets
    /// </summary>
    /// <returns>List of default parameter sets</returns>
    /// <response code="200">Returns the list of default parameter sets</response>
    [HttpGet("default")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ParameterSetDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParameterSetDto>>>> GetDefaultParameterSets()
    {
        try
        {
            var parameterSets = await _parameterSetService.GetDefaultParameterSetsAsync();

            var response = ApiResponse<IEnumerable<ParameterSetDto>>.SuccessResponse(
                parameterSets,
                "Default parameter sets retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving default parameter sets");
            var response = ApiResponse<IEnumerable<ParameterSetDto>>.ErrorResponse(
                "An error occurred while retrieving default parameter sets"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Get a specific parameter set by ID
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <returns>The parameter set details</returns>
    /// <response code="200">Returns the parameter set</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the parameter set</response>
    /// <response code="404">If parameter set not found</response>
    [HttpGet("{paramSetId}")]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ParameterSetDto>>> GetParameterSet(int paramSetId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var parameterSet = await _parameterSetService.GetParameterSetByIdAsync(paramSetId, userId);

            if (parameterSet == null)
            {
                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse("Parameter set not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<ParameterSetDto>.SuccessResponse(
                parameterSet,
                "Parameter set retrieved successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for parameter set {ParamSetId}", paramSetId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter set {ParamSetId}", paramSetId);
            var response = ApiResponse<ParameterSetDto>.ErrorResponse(
                "An error occurred while retrieving the parameter set"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Create a new parameter set
    /// </summary>
    /// <param name="createDto">The parameter set creation data</param>
    /// <returns>The created parameter set</returns>
    /// <response code="201">Returns the created parameter set</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ParameterSetDto>>> CreateParameterSet(CreateParameterSetDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var parameterSet = await _parameterSetService.CreateParameterSetAsync(createDto, userId);

            var response = ApiResponse<ParameterSetDto>.SuccessResponse(
                parameterSet,
                "Parameter set created successfully"
            );

            return CreatedAtAction(
                nameof(GetParameterSet),
                new { paramSetId = parameterSet.ParamSetId },
                response
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for parameter set creation");
            var response = ApiResponse<ParameterSetDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter set");
            var response = ApiResponse<ParameterSetDto>.ErrorResponse(
                "An error occurred while creating the parameter set"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Update an existing parameter set
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <param name="updateDto">The parameter set update data</param>
    /// <returns>The updated parameter set</returns>
    /// <response code="200">Returns the updated parameter set</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the parameter set</response>
    /// <response code="404">If parameter set not found</response>
    [HttpPut("{paramSetId}")]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ParameterSetDto>>> UpdateParameterSet(int paramSetId, UpdateParameterSetDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var parameterSet = await _parameterSetService.UpdateParameterSetAsync(updateDto, userId);

            var response = ApiResponse<ParameterSetDto>.SuccessResponse(
                parameterSet,
                "Parameter set updated successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for parameter set {ParamSetId}", paramSetId);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parameter set {ParamSetId} not found", paramSetId);
            var response = ApiResponse<ParameterSetDto>.ErrorResponse("Parameter set not found");
            return NotFound(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for parameter set {ParamSetId} update", paramSetId);
            var response = ApiResponse<ParameterSetDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parameter set {ParamSetId}", paramSetId);
            var response = ApiResponse<ParameterSetDto>.ErrorResponse(
                "An error occurred while updating the parameter set"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Delete a parameter set
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Parameter set deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the parameter set</response>
    /// <response code="404">If parameter set not found</response>
    [HttpDelete("{paramSetId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteParameterSet(int paramSetId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _parameterSetService.DeleteParameterSetAsync(paramSetId, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Parameter set not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Parameter set deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for parameter set {ParamSetId}", paramSetId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter set {ParamSetId}", paramSetId);
            var response = ApiResponse<object>.ErrorResponse(
                "An error occurred while deleting the parameter set"
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