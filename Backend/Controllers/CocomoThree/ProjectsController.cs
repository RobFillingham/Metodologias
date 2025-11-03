using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Controllers.CocomoThree;

/// <summary>
/// Controller for managing COCOMO projects
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO II - Projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IProjectService projectService,
        ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects for the current user
    /// </summary>
    /// <returns>List of user's projects</returns>
    /// <response code="200">Returns the list of projects</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDto>>>> GetUserProjects()
    {
        try
        {
            var userId = GetCurrentUserId();
            var projects = await _projectService.GetUserProjectsAsync(userId);

            var response = ApiResponse<IEnumerable<ProjectDto>>.SuccessResponse(
                projects,
                "Projects retrieved successfully"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user projects");
            throw;
        }
    }

    /// <summary>
    /// Get a specific project by ID
    /// </summary>
    /// <param name="id">The project ID</param>
    /// <returns>The project details</returns>
    /// <response code="200">Returns the project</response>
    /// <response code="404">If project not found or not accessible</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> GetProjectById(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var project = await _projectService.GetProjectByIdAsync(id, userId);

            if (project == null)
            {
                var errorResponse = ApiResponse<ProjectDto>.ErrorResponse(
                    "Project not found or access denied",
                    new List<string> { "The project does not exist or you don't have permission to access it" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<ProjectDto>.SuccessResponse(
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
    /// <param name="createDto">The project creation data</param>
    /// <returns>The created project</returns>
    /// <response code="201">Project created successfully</response>
    /// <response code="400">If input validation fails</response>
    /// <response code="401">If user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> CreateProject([FromBody] CreateProjectDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ProjectDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var project = await _projectService.CreateProjectAsync(createDto, userId);

            var response = ApiResponse<ProjectDto>.SuccessResponse(
                project,
                "Project created successfully"
            );

            return CreatedAtAction(nameof(GetProjectById), new { id = project.ProjectId }, response);
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
    /// <param name="id">The project ID</param>
    /// <param name="updateDto">The project update data</param>
    /// <returns>The updated project</returns>
    /// <response code="200">Project updated successfully</response>
    /// <response code="400">If input validation fails or ID mismatch</response>
    /// <response code="404">If project not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> UpdateProject(int id, [FromBody] UpdateProjectDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<ProjectDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            if (id != updateDto.ProjectId)
            {
                var errorResponse = ApiResponse<ProjectDto>.ErrorResponse(
                    "ID mismatch",
                    new List<string> { "The project ID in the URL does not match the ID in the request body" }
                );
                return BadRequest(errorResponse);
            }

            var userId = GetCurrentUserId();
            var project = await _projectService.UpdateProjectAsync(updateDto, userId);

            var response = ApiResponse<ProjectDto>.SuccessResponse(
                project,
                "Project updated successfully"
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project not found: {ProjectId}", id);
            var errorResponse = ApiResponse<ProjectDto>.ErrorResponse(
                "Project not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to project: {ProjectId}", id);
            var errorResponse = ApiResponse<ProjectDto>.ErrorResponse(
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
    /// <param name="id">The project ID</param>
    /// <returns>Success response</returns>
    /// <response code="200">Project deleted successfully</response>
    /// <response code="404">If project not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't own the project</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProject(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _projectService.DeleteProjectAsync(id, userId);

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
