using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoTwoStageOne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.CocomoTwoStageOne;

/// <summary>
/// Controller for managing Parameter Sets (COCOMO 2 Stage 1)
/// </summary>
[ApiController]
[Route("api/cocomo2-stage1/parameter-sets")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 1 - Parameter Sets")]
public class ParameterSetsCocomo2Stage1Controller : ControllerBase
{
    private readonly IParameterSetCocomo2Stage1Service _service;
    private readonly ILogger<ParameterSetsCocomo2Stage1Controller> _logger;

    public ParameterSetsCocomo2Stage1Controller(
        IParameterSetCocomo2Stage1Service service,
        ILogger<ParameterSetsCocomo2Stage1Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get user's custom parameter sets
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ParameterSetCocomo2Stage1Dto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParameterSetCocomo2Stage1Dto>>>> GetMySets()
    {
        try
        {
            var userId = GetCurrentUserId();
            var sets = await _service.GetUserSetsAsync(userId);

            var response = ApiResponse<IEnumerable<ParameterSetCocomo2Stage1Dto>>.SuccessResponse(
                sets,
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
    /// Get default (system) parameter sets
    /// </summary>
    [HttpGet("default")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ParameterSetCocomo2Stage1Dto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParameterSetCocomo2Stage1Dto>>>> GetDefaultSets()
    {
        try
        {
            var sets = await _service.GetDefaultSetsAsync();

            var response = ApiResponse<IEnumerable<ParameterSetCocomo2Stage1Dto>>.SuccessResponse(
                sets,
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
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ParameterSetCocomo2Stage1Dto>>> GetById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var paramSet = await _service.GetByIdAsync(id, userId);

            if (paramSet == null)
            {
                var errorResponse = ApiResponse<ParameterSetCocomo2Stage1Dto>.ErrorResponse(
                    "Parameter set not found",
                    new List<string> { "The parameter set does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<ParameterSetCocomo2Stage1Dto>.SuccessResponse(
                paramSet,
                "Parameter set retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parameter set {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new custom parameter set
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ParameterSetCocomo2Stage1Dto>>> Create([FromBody] CreateParameterSetCocomo2Stage1Dto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ParameterSetCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var paramSet = await _service.CreateAsync(createDto, userId);

            var response = ApiResponse<ParameterSetCocomo2Stage1Dto>.SuccessResponse(
                paramSet,
                "Parameter set created successfully"
            );

            return CreatedAtAction(nameof(GetById), new { id = paramSet.ParamSetId }, response);
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
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ParameterSetCocomo2Stage1Dto>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ParameterSetCocomo2Stage1Dto>>> Update(int id, [FromBody] CreateParameterSetCocomo2Stage1Dto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ParameterSetCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var paramSet = await _service.UpdateAsync(id, updateDto, userId);

            var response = ApiResponse<ParameterSetCocomo2Stage1Dto>.SuccessResponse(
                paramSet,
                "Parameter set updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parameter set not found: {Id}", id);
            var errorResponse = ApiResponse<ParameterSetCocomo2Stage1Dto>.ErrorResponse(
                "Parameter set not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on parameter set: {Id}", id);
            var errorResponse = ApiResponse<ParameterSetCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation on parameter set: {Id}", id);
            var errorResponse = ApiResponse<ParameterSetCocomo2Stage1Dto>.ErrorResponse(
                "Invalid operation",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parameter set {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a parameter set
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _service.DeleteAsync(id, userId);

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
            _logger.LogWarning(ex, "Unauthorized delete attempt on parameter set: {Id}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation on parameter set: {Id}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Invalid operation",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter set {Id}", id);
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
