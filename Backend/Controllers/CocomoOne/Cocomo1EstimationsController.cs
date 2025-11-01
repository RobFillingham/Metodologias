using Backend.Models.DTOs.CocomoOne;
using Backend.Models.Responses;
using Backend.Services.Interfaces.CocomoOne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.CocomoOne;

[ApiController]
[Route("api/cocomo1-estimations")]
[Authorize]
[Produces("application/json")]
[Tags("COCOMO I - Estimations")]
public class Cocomo1EstimationsController : ControllerBase
{
    private readonly ICocomo1EstimationService _service;
    private readonly ILogger<Cocomo1EstimationsController> _logger;

    public Cocomo1EstimationsController(ICocomo1EstimationService service, ILogger<Cocomo1EstimationsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todas las estimaciones de un proyecto
    /// </summary>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Cocomo1EstimationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<Cocomo1EstimationDto>>>> GetByProject(int projectId)
    {
        try
        {
            var result = await _service.GetByProjectIdAsync(projectId);
            var response = ApiResponse<IEnumerable<Cocomo1EstimationDto>>.SuccessResponse(
                result,
                "COCOMO 1 Estimations retrieved successfully"
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving COCOMO 1 estimations for project {ProjectId}", projectId);
            var errorResponse = ApiResponse<IEnumerable<Cocomo1EstimationDto>>.ErrorResponse(
                "Error retrieving estimations",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Obtener una estimación por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Cocomo1EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cocomo1EstimationDto>>> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                    "Estimation not found",
                    new List<string> { "The requested COCOMO 1 estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            var response = ApiResponse<Cocomo1EstimationDto>.SuccessResponse(
                result,
                "COCOMO 1 Estimation retrieved successfully"
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving COCOMO 1 estimation {EstimationId}", id);
            var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                "Error retrieving estimation",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Crear una nueva estimación COCOMO 1
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Cocomo1EstimationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cocomo1EstimationDto>>> Create([FromBody] CreateCocomo1EstimationDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var created = await _service.CreateAsync(dto);
            var response = ApiResponse<Cocomo1EstimationDto>.SuccessResponse(
                created,
                "COCOMO 1 Estimation created successfully"
            );

            return CreatedAtAction(nameof(GetById), new { id = created.Cocomo1EstimationId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating COCOMO 1 estimation");
            var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                "Error creating estimation",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Actualizar una estimación
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Cocomo1EstimationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<Cocomo1EstimationDto>>> Update(int id, [FromBody] CreateCocomo1EstimationDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                    "Validation failed",
                    errors
                );
                return BadRequest(errorResponse);
            }

            var updated = await _service.UpdateAsync(id, dto);
            var response = ApiResponse<Cocomo1EstimationDto>.SuccessResponse(
                updated,
                "COCOMO 1 Estimation updated successfully"
            );
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "COCOMO 1 Estimation not found: {EstimationId}", id);
            var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                "Estimation not found",
                new List<string> { ex.Message }
            );
            return NotFound(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating COCOMO 1 estimation {EstimationId}", id);
            var errorResponse = ApiResponse<Cocomo1EstimationDto>.ErrorResponse(
                "Error updating estimation",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Eliminar una estimación
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(
                    "Estimation not found",
                    new List<string> { "The requested COCOMO 1 estimation does not exist" }
                );
                return NotFound(errorResponse);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting COCOMO 1 estimation {EstimationId}", id);
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "Error deleting estimation",
                new List<string> { ex.Message }
            );
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}
