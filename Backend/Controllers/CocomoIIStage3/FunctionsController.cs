using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Controllers.CocomoIIStage3;

/// <summary>
/// Controller for managing estimation functions (function points)
/// </summary>
[ApiController]
[Route("api/Estimations/{estimationId}/CocomoIIStage3/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 3 - Functions")]
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
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for estimation {EstimationId}", estimationId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving functions for estimation {EstimationId}", estimationId);
            var response = ApiResponse<IEnumerable<EstimationFunctionDto>>.ErrorResponse(
                "An error occurred while retrieving functions"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Get a specific function by ID
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="functionId">The function ID</param>
    /// <returns>The function details</returns>
    /// <response code="200">Returns the function</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    /// <response code="404">If function not found</response>
    [HttpGet("{functionId}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EstimationFunctionDto>>> GetFunction(int estimationId, int functionId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var function = await _functionService.GetFunctionByIdAsync(functionId, userId);

            if (function == null)
            {
                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse("Function not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<EstimationFunctionDto>.SuccessResponse(
                function,
                "Function retrieved successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for function {FunctionId}", functionId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving function {FunctionId}", functionId);
            var response = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "An error occurred while retrieving the function"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Create a new function for an estimation
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="createDto">The function creation data</param>
    /// <returns>The created function</returns>
    /// <response code="201">Returns the created function</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EstimationFunctionDto>>> CreateFunction(int estimationId, CreateEstimationFunctionDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var function = await _functionService.CreateFunctionAsync(estimationId, createDto, userId);

            var response = ApiResponse<EstimationFunctionDto>.SuccessResponse(
                function,
                "Function created successfully"
            );

            return CreatedAtAction(
                nameof(GetFunction),
                new { estimationId, functionId = function.FunctionId },
                response
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for estimation {EstimationId}", estimationId);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for function creation in estimation {EstimationId}", estimationId);
            var response = ApiResponse<EstimationFunctionDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating function for estimation {EstimationId}", estimationId);
            var response = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "An error occurred while creating the function"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Update an existing function
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="functionId">The function ID</param>
    /// <param name="updateDto">The function update data</param>
    /// <returns>The updated function</returns>
    /// <response code="200">Returns the updated function</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    /// <response code="404">If function not found</response>
    [HttpPut("{functionId}")]
    [ProducesResponseType(typeof(ApiResponse<EstimationFunctionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EstimationFunctionDto>>> UpdateFunction(int estimationId, int functionId, UpdateEstimationFunctionDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ApiResponse<EstimationFunctionDto>.ErrorResponse("Invalid request data");
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var function = await _functionService.UpdateFunctionAsync(functionId, updateDto, userId);

            var response = ApiResponse<EstimationFunctionDto>.SuccessResponse(
                function,
                "Function updated successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for function {FunctionId}", functionId);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Function {FunctionId} not found", functionId);
            var response = ApiResponse<EstimationFunctionDto>.ErrorResponse("Function not found");
            return NotFound(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data for function {FunctionId} update", functionId);
            var response = ApiResponse<EstimationFunctionDto>.ErrorResponse(ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating function {FunctionId}", functionId);
            var response = ApiResponse<EstimationFunctionDto>.ErrorResponse(
                "An error occurred while updating the function"
            );
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    /// <summary>
    /// Delete a function
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <param name="functionId">The function ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Function deleted successfully</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the estimation</response>
    /// <response code="404">If function not found</response>
    [HttpDelete("{functionId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFunction(int estimationId, int functionId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _functionService.DeleteFunctionAsync(functionId, userId);

            if (!result)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Function not found");
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null,
                "Function deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for function {FunctionId}", functionId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting function {FunctionId}", functionId);
            var response = ApiResponse<object>.ErrorResponse(
                "An error occurred while deleting the function"
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