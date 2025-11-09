using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoTwoStageOne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.CocomoTwoStageOne;

/// <summary>
/// Controller for managing Components (COCOMO 2 Stage 1)
/// </summary>
[ApiController]
[Route("api/cocomo2-stage1/estimations/{estimationId}/components")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 1 - Components")]
public class ComponentsCocomo2Stage1Controller : ControllerBase
{
    private readonly IComponentCocomo2Stage1Service _service;
    private readonly ILogger<ComponentsCocomo2Stage1Controller> _logger;

    public ComponentsCocomo2Stage1Controller(
        IComponentCocomo2Stage1Service service,
        ILogger<ComponentsCocomo2Stage1Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get all components for an estimation
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>>> GetEstimationComponents(int estimationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var components = await _service.GetEstimationComponentsAsync(estimationId, userId);

            var response = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.SuccessResponse(
                components,
                "Components retrieved successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to estimation components: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving components for estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Get a specific component by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ComponentCocomo2Stage1Dto>>> GetComponentById(int estimationId, int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var component = await _service.GetByIdAsync(id, userId);

            if (component == null)
            {
                var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                    "Component not found or access denied",
                    new List<string> { "The component does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<ComponentCocomo2Stage1Dto>.SuccessResponse(
                component,
                "Component retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving component {ComponentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Add a new component and recalculate estimation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ComponentCocomo2Stage1Dto>>> CreateComponent(
        int estimationId,
        [FromBody] CreateComponentCocomo2Stage1Dto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var component = await _service.CreateAsync(estimationId, createDto, userId);

            var response = ApiResponse<ComponentCocomo2Stage1Dto>.SuccessResponse(
                component,
                "Component created and estimation recalculated successfully"
            );

            return CreatedAtAction(
                nameof(GetComponentById),
                new { estimationId = estimationId, id = component.ComponentId },
                response
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized component creation in estimation: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating component for estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Add multiple components in batch and recalculate once
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>>> CreateBatchComponents(
        int estimationId,
        [FromBody] CreateBatchComponentsCocomo2Stage1Dto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var components = await _service.CreateBatchAsync(estimationId, createDto, userId);

            var response = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.SuccessResponse(
                components,
                $"{components.Count()} components created and estimation recalculated successfully"
            );

            return CreatedAtAction(
                nameof(GetEstimationComponents),
                new { estimationId = estimationId },
                response
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized batch component creation in estimation: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<ComponentCocomo2Stage1Dto>>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch components for estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Update an existing component and recalculate estimation
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ComponentCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ComponentCocomo2Stage1Dto>>> UpdateComponent(
        int estimationId,
        int id,
        [FromBody] UpdateComponentCocomo2Stage1Dto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var component = await _service.UpdateAsync(id, updateDto, userId);

            var response = ApiResponse<ComponentCocomo2Stage1Dto>.SuccessResponse(
                component,
                "Component updated and estimation recalculated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Component not found: {ComponentId}", id);
            var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                "Component not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on component: {ComponentId}", id);
            var errorResponse = ApiResponse<ComponentCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating component {ComponentId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a component and recalculate estimation
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteComponent(int estimationId, int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _service.DeleteAsync(id, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Component not found",
                    new List<string> { "The component does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Component deleted and estimation recalculated successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized delete attempt on component: {ComponentId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting component {ComponentId}", id);
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
