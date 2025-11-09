using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Controllers.CocomoThree;

/// <summary>
/// Controller for managing COCOMO II Parameter Sets
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II - Parameter Sets")]
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
            _logger.LogError(ex, "Error retrieving user parameter sets");
            throw;
        }
    }

    /// <summary>
    /// Get all default (system) parameter sets
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
            throw;
        }
    }

    /// <summary>
    /// Get a specific parameter set by ID
    /// </summary>
    /// <param name="id">The parameter set ID</param>
    /// <returns>The parameter set</returns>
    /// <response code="200">Returns the parameter set</response>
    /// <response code="404">If parameter set not found or not accessible</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ParameterSetDto>>> GetParameterSet(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var parameterSet = await _parameterSetService.GetParameterSetByIdAsync(id, userId);

            if (parameterSet == null)
            {
                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                    "Parameter set not found or access denied",
                    new List<string> { "The parameter set does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<ParameterSetDto>.SuccessResponse(
                parameterSet,
                "Parameter set retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter set {ParameterSetId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new parameter set
    /// </summary>
    /// <param name="createDto">The parameter set creation data</param>
    /// <returns>The created parameter set</returns>
    /// <response code="201">Parameter set created successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ParameterSetDto>>> CreateParameterSet([FromBody] CreateParameterSetDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var parameterSet = await _parameterSetService.CreateParameterSetAsync(createDto, userId);

            var response = ApiResponse<ParameterSetDto>.SuccessResponse(
                parameterSet,
                "Parameter set created successfully"
            );

            return CreatedAtAction(nameof(GetParameterSet), new { id = parameterSet.ParamSetId }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error during parameter set creation");
            var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation during parameter set creation");
            var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter set");
            throw;
        }
    }

    /// <summary>
    /// Update an existing parameter set
    /// </summary>
    /// <param name="id">The parameter set ID</param>
    /// <param name="updateDto">The parameter set update data</param>
    /// <returns>The updated parameter set</returns>
    /// <response code="200">Parameter set updated successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="404">If parameter set not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the parameter set</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ParameterSetDto>>> UpdateParameterSet(int id, [FromBody] UpdateParameterSetDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            // Ensure the ID in the URL matches the ID in the DTO
            if (id != updateDto.ParamSetId)
            {
                var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                    "ID mismatch",
                    new List<string> { "The parameter set ID in the URL does not match the ID in the request body" }
                );
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
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parameter set not found: {ParameterSetId}", id);
            var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                "Parameter set not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to parameter set: {ParameterSetId}", id);
            var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error during parameter set update: {ParameterSetId}", id);
            var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation during parameter set update: {ParameterSetId}", id);
            var errorResponse = ApiResponse<ParameterSetDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parameter set {ParameterSetId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a parameter set
    /// </summary>
    /// <param name="id">The parameter set ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Parameter set deleted successfully</response>
    /// <response code="404">If parameter set not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the parameter set</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteParameterSet(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _parameterSetService.DeleteParameterSetAsync(id, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Parameter set not found",
                    new List<string> { "The parameter set does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Parameter set deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to delete parameter set: {ParameterSetId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation when deleting parameter set: {ParameterSetId}. Message: {Message}", id, ex.Message);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Cannot delete parameter set",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter set {ParameterSetId}", id);
            throw;
        }
    }

    /// <summary>
    /// Extract the current user ID from the JWT token
    /// </summary>
    private int GetCurrentUserId()
    {
        // Try both the standard claim name and the mapped claim name
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