using Microsoft.AspNetCore.Mvc;
using Backend.Models.Responses;

namespace Backend.Controllers;

/// <summary>
/// Sample controller to demonstrate API structure
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SampleController : ControllerBase
{
    private readonly ILogger<SampleController> _logger;

    public SampleController(ILogger<SampleController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get a sample greeting message
    /// </summary>
    /// <returns>A greeting message</returns>
    /// <response code="200">Returns a successful response with greeting</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<string>> Get()
    {
        _logger.LogInformation("Sample GET endpoint called");
        var response = ApiResponse<string>.SuccessResponse(
            "Hello from Backend API!", 
            "Sample endpoint working"
        );
        return Ok(response);
    }

    /// <summary>
    /// Get a greeting message by name
    /// </summary>
    /// <param name="name">The name to greet</param>
    /// <returns>A personalized greeting message</returns>
    /// <response code="200">Returns a successful response with personalized greeting</response>
    /// <response code="400">If the name is empty or null</response>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<string>> GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            var errorResponse = ApiResponse<string>.ErrorResponse(
                "Name cannot be empty",
                new List<string> { "Please provide a valid name" }
            );
            return BadRequest(errorResponse);
        }

        _logger.LogInformation("Sample GET by name endpoint called with name: {Name}", name);
        var response = ApiResponse<string>.SuccessResponse(
            $"Hello, {name}!",
            "Personalized greeting"
        );
        return Ok(response);
    }

    /// <summary>
    /// Create a sample item (POST example)
    /// </summary>
    /// <param name="data">Sample data to create</param>
    /// <returns>Created item confirmation</returns>
    /// <response code="201">Returns confirmation of creation</response>
    /// <response code="400">If the data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<object>> Post([FromBody] SampleData data)
    {
        if (data == null || string.IsNullOrWhiteSpace(data.Value))
        {
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Invalid data",
                new List<string> { "Value is required" }
            );
            return BadRequest(errorResponse);
        }

        _logger.LogInformation("Sample POST endpoint called with value: {Value}", data.Value);
        var result = new { Id = Guid.NewGuid(), Value = data.Value, CreatedAt = DateTime.UtcNow };
        var response = ApiResponse<object>.SuccessResponse(result, "Item created successfully");
        
        return CreatedAtAction(nameof(Get), response);
    }
}

/// <summary>
/// Sample data model for POST requests
/// </summary>
public class SampleData
{
    /// <summary>
    /// The value of the sample data
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
