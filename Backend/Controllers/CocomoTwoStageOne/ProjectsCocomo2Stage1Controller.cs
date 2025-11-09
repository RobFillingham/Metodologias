using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoTwoStageOne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.CocomoTwoStageOne;

/// <summary>
/// Controller for managing Projects (COCOMO 2 Stage 1)
/// </summary>
[ApiController]
[Route("api/cocomo2-stage1/projects")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II Stage 1 - Projects")]
public class ProjectsCocomo2Stage1Controller : ControllerBase
{
    private readonly IProjectCocomo2Stage1Service _service;
    private readonly ILogger<ProjectsCocomo2Stage1Controller> _logger;

    public ProjectsCocomo2Stage1Controller(
        IProjectCocomo2Stage1Service service,
        ILogger<ProjectsCocomo2Stage1Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectCocomo2Stage1Dto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectCocomo2Stage1Dto>>>> GetUserProjects()
    {
        try
        {
            var userId = GetCurrentUserId();
            var projects = await _service.GetUserProjectsAsync(userId);

            var response = ApiResponse<IEnumerable<ProjectCocomo2Stage1Dto>>.SuccessResponse(
                projects,
                "Projects retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            throw;
        }
    }

    /// <summary>
    /// Get a specific project by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProjectCocomo2Stage1Dto>>> GetById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var project = await _service.GetByIdAsync(id, userId);

            if (project == null)
            {
                var errorResponse = ApiResponse<ProjectCocomo2Stage1Dto>.ErrorResponse(
                    "Project not found or access denied",
                    new List<string> { "The project does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<ProjectCocomo2Stage1Dto>.SuccessResponse(
                project,
                "Project retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProjectCocomo2Stage1Dto>>> Create([FromBody] CreateProjectCocomo2Stage1Dto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ProjectCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var project = await _service.CreateAsync(createDto, userId);

            var response = ApiResponse<ProjectCocomo2Stage1Dto>.SuccessResponse(
                project,
                "Project created successfully"
            );

            return CreatedAtAction(nameof(GetById), new { id = project.ProjectId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            throw;
        }
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ProjectCocomo2Stage1Dto>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ProjectCocomo2Stage1Dto>>> Update(int id, [FromBody] UpdateProjectCocomo2Stage1Dto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ProjectCocomo2Stage1Dto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var project = await _service.UpdateAsync(id, updateDto, userId);

            var response = ApiResponse<ProjectCocomo2Stage1Dto>.SuccessResponse(
                project,
                "Project updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project not found: {ProjectId}", id);
            var errorResponse = ApiResponse<ProjectCocomo2Stage1Dto>.ErrorResponse(
                "Project not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized update attempt on project: {ProjectId}", id);
            var errorResponse = ApiResponse<ProjectCocomo2Stage1Dto>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project {ProjectId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a project
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
                    "Project not found",
                    new List<string> { "The project does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<object>.SuccessResponse(
                null!,
                "Project deleted successfully"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized delete attempt on project: {ProjectId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Access denied",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project {ProjectId}", id);
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
