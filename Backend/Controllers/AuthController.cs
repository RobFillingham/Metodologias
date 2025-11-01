using Microsoft.AspNetCore.Mvc;
using Backend.Models.DTOs;
using Backend.Models.Responses;
using Backend.Services.Interfaces;

namespace Backend.Controllers;

/// <summary>
/// Authentication controller for login and signup
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>Authentication response with JWT token</returns>
    /// <response code="201">User registered successfully</response>
    /// <response code="400">Invalid input or email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var result = await _authService.RegisterAsync(registerDto);
            var response = ApiResponse<AuthResponseDto>.SuccessResponse(
                result,
                "User registered successfully"
            );

            return CreatedAtAction(nameof(Register), response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed");
            var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                ex.Message,
                new List<string> { ex.Message }
            );
            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            throw;
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var result = await _authService.LoginAsync(loginDto);
            var response = ApiResponse<AuthResponseDto>.SuccessResponse(
                result,
                "Login successful"
            );

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed");
            var errorResponse = ApiResponse<AuthResponseDto>.ErrorResponse(
                "Invalid credentials",
                new List<string> { ex.Message }
            );
            return Unauthorized(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw;
        }
    }
}
