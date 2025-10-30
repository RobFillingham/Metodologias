using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Controllers.CocomoThree;

/// <summary>
/// Controller for managing estimation functions (function points)
/// </summary>
[ApiController]
[Route("api/Estimations/{estimationId}/[controller]")]
[Authorize]
[Produces("application/json")]
public class FunctionsController : ControllerBase
{
    private readonly IEstimationFunctionService _functionService;
    private readonly ILogger<FunctionsController> _logger;

    public FunctionsController(
        IEstimationFunctionService functionService,
        ILogger<FunctionsController> logger)
    {
        _functionService = functionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all functions for an estimation
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <returns>List of functions with calculated complexity and points</returns>
    /// <response code="200">Returns the list of functions</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationFunctionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EstimationFunctionDto>>>> GetEstimationFunctions(int estimationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var functions = await _functionService.GetEstimationFunctionsAsync(estimationId, userId);

            var response = ApiResponse<IEnumerable<EstimationFunctionDto>>.SuccessResponse(
                functions,
                "Functions retrieved successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to estimation functions: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving functions for estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Get a specific function by ID
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="id">The function ID</param>
    /// <returns>The function details</returns>
    /// <response code="200">Returns the function</response>
    /// <response code="404">If function not found or not accessible</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<EstimationFunctionDto>>> GetFunctionById(int estimationId, int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var function = await _functionService.GetFunctionByIdAsync(id, userId);

            if (function == null)
            {
                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                    "Function not found or access denied",
                    new List<string> { "The function does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<EstimationFunctionDto>.SuccessResponse(
                function,
                "Function retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving function {FunctionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new function (calculates complexity and recalculates estimation)
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="createDto">The function creation data</param>
    /// <returns>The created function with calculated complexity and points</returns>
    /// <response code="201">Function created successfully and estimation recalculated</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationFunctionDto>>> CreateFunction(
        int estimationId, 
        [FromBody] CreateEstimationFunctionDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var function = await _functionService.CreateFunctionAsync(estimationId, createDto, userId);

            var response = ApiResponse<EstimationFunctionDto>.SuccessResponse(
                function,
                "Function created successfully and estimation recalculated"
            );

            return CreatedAtAction(
                nameof(GetFunctionById), 
                new { estimationId = estimationId, id = function.FunctionId }, 
                response
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized function creation in estimation: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid function data");
            var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "Invalid data",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating function for estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Create multiple functions in batch (optimized - recalculates estimation only once)
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="batchDto">The batch of functions to create</param>
    /// <returns>The created functions with calculated complexity and points</returns>
    /// <response code="201">Functions created successfully and estimation recalculated</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpPost("Batch")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationFunctionDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationFunctionDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EstimationFunctionDto>>>> CreateBatchFunctions(
        int estimationId, 
        [FromBody] CreateBatchEstimationFunctionsDto batchDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var functions = await _functionService.CreateBatchFunctionsAsync(estimationId, batchDto, userId);

            var response = ApiResponse<IEnumerable<EstimationFunctionDto>>.SuccessResponse(
                functions,
                $"{functions.Count()} functions created successfully and estimation recalculated"
            );

            return CreatedAtAction(
                nameof(GetEstimationFunctions), 
                new { estimationId = estimationId }, 
                response
            );
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Estimation not found: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized batch function creation in estimation: {EstimationId}", estimationId);
            var errorResponse = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid function data in batch");
            var errorResponse = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                "Invalid data",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch functions for estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Update a function (recalculates complexity and recalculates estimation)
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="id">The function ID</param>
    /// <param name="updateDto">The function update data</param>
    /// <returns>The updated function with recalculated complexity and points</returns>
    /// <response code="200">Function updated successfully and estimation recalculated</response>
    /// <response code="400">If input validation fails or ID mismatch</response>
    /// <response code="404">If function not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the function</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationFunctionDto>>> UpdateFunction(
        int estimationId, 
        int id, 
        [FromBody] UpdateEstimationFunctionDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            if (id != updateDto.FunctionId)
            {
                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                    "ID mismatch",
                    new List<string> { "The function ID in the URL does not match the ID in the request body" }
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var function = await _functionService.UpdateFunctionAsync(id, updateDto, userId);

            var response = ApiResponse<EstimationFunctionDto>.SuccessResponse(
                function,
                "Function updated successfully and estimation recalculated"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Function not found: {FunctionId}", id);
            var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "Function not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on function: {FunctionId}", id);
            var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid function data");
            var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "Invalid data",
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating function {FunctionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a function (recalculates estimation)
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="id">The function ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Function deleted successfully and estimation recalculated</response>
    /// <response code="404">If function not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the function</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFunction(int estimationId, int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _functionService.DeleteFunctionAsync(id, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Function not found",
                    new List<string> { "The function does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Function deleted successfully and estimation recalculated"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized delete attempt on function: {FunctionId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting function {FunctionId}", id);
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
