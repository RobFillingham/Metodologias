using Backend.Models.DTOs;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

/// <summary>
/// Service implementation for KLOC Estimation operations
/// KLOC method: Estimates effort, time and cost based on Lines of Code
/// </summary>
public class KlocEstimationService : IKlocEstimationService
{
    private readonly IKlocEstimationRepository _repository;
    private readonly ILogger<KlocEstimationService> _logger;

    // KLOC estimation constants (can be moved to database/config)
    private const decimal EffortPerKloc = 2.9m;      // Effort in person-months per KLOC
    private const decimal CostPerPersonMonth = 8000m; // Cost per person-month in currency units
    private const decimal TimeCoefficient = 0.38m;    // Time factor based on effort

    public KlocEstimationService(
        IKlocEstimationRepository repository,
        ILogger<KlocEstimationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<KlocEstimationDto>> GetEstimationsByProjectAsync(int projectId)
    {
        try
        {
            var estimations = await _repository.GetEstimationsByProjectAsync(projectId);
            return estimations.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving KLOC estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<KlocEstimationDto?> GetEstimationByIdAsync(int estimationId)
    {
        try
        {
            var estimation = await _repository.GetEstimationByIdAsync(estimationId);
            return estimation != null ? MapToDto(estimation) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving KLOC estimation {EstimationId}", estimationId);
            throw;
        }
    }

    public async Task<KlocEstimationDto> CreateEstimationAsync(CreateKlocEstimationDto createDto)
    {
        try
        {
            // Convert lines of code to KLOC (thousands of lines)
            decimal kloc = createDto.LinesOfCode / 1000m;

            // Calculate effort (person-months)
            decimal estimatedEffort = CalculateEffort(kloc);

            // Calculate estimated time (months)
            decimal estimatedTime = CalculateTime(estimatedEffort);

            // Calculate estimated cost
            decimal estimatedCost = CalculateCost(estimatedEffort);

            var estimation = new KlocEstimation
            {
                ProjectId = createDto.ProjectId,
                EstimationName = createDto.EstimationName,
                LinesOfCode = createDto.LinesOfCode,
                EstimatedEffort = Math.Round(estimatedEffort, 2),
                EstimatedTime = Math.Round(estimatedTime, 2),
                EstimatedCost = Math.Round(estimatedCost, 2),
                Notes = createDto.Notes
            };

            var createdEstimation = await _repository.CreateEstimationAsync(estimation);

            _logger.LogInformation("KLOC estimation created: {EstimationId} for project {ProjectId}", 
                createdEstimation.KlocEstimationId, createDto.ProjectId);

            return MapToDto(createdEstimation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating KLOC estimation for project {ProjectId}", createDto.ProjectId);
            throw;
        }
    }

    public async Task<KlocEstimationDto> UpdateEstimationAsync(UpdateKlocEstimationDto updateDto)
    {
        try
        {
            var estimation = await _repository.GetEstimationByIdAsync(updateDto.KlocEstimationId);
            if (estimation == null)
            {
                throw new KeyNotFoundException($"KLOC estimation with ID {updateDto.KlocEstimationId} not found");
            }

            // Recalculate metrics based on new lines of code
            decimal kloc = updateDto.LinesOfCode / 1000m;
            decimal estimatedEffort = CalculateEffort(kloc);
            decimal estimatedTime = CalculateTime(estimatedEffort);
            decimal estimatedCost = CalculateCost(estimatedEffort);

            estimation.EstimationName = updateDto.EstimationName;
            estimation.LinesOfCode = updateDto.LinesOfCode;
            estimation.EstimatedEffort = Math.Round(estimatedEffort, 2);
            estimation.EstimatedTime = Math.Round(estimatedTime, 2);
            estimation.EstimatedCost = Math.Round(estimatedCost, 2);
            estimation.Notes = updateDto.Notes;

            var updatedEstimation = await _repository.UpdateEstimationAsync(estimation);

            _logger.LogInformation("KLOC estimation updated: {EstimationId}", updateDto.KlocEstimationId);

            return MapToDto(updatedEstimation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating KLOC estimation {EstimationId}", updateDto.KlocEstimationId);
            throw;
        }
    }

    public async Task<bool> DeleteEstimationAsync(int estimationId)
    {
        try
        {
            var result = await _repository.DeleteEstimationAsync(estimationId);
            if (result)
            {
                _logger.LogInformation("KLOC estimation deleted: {EstimationId}", estimationId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting KLOC estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Calculate effort in person-months using KLOC method
    /// Formula: Effort = EffortPerKloc * KLOC
    /// </summary>
    private decimal CalculateEffort(decimal kloc)
    {
        return EffortPerKloc * kloc;
    }

    /// <summary>
    /// Calculate estimated time in months
    /// Formula: Time = 2.5 * (Effort ^ 0.38)
    /// </summary>
    private decimal CalculateTime(decimal effort)
    {
        return 2.5m * (decimal)Math.Pow((double)effort, (double)TimeCoefficient);
    }

    /// <summary>
    /// Calculate estimated cost
    /// Formula: Cost = Effort * CostPerPersonMonth
    /// </summary>
    private decimal CalculateCost(decimal effort)
    {
        return effort * CostPerPersonMonth;
    }

    /// <summary>
    /// Map KlocEstimation entity to DTO
    /// </summary>
    private KlocEstimationDto MapToDto(KlocEstimation estimation)
    {
        return new KlocEstimationDto
        {
            KlocEstimationId = estimation.KlocEstimationId,
            ProjectId = estimation.ProjectId,
            EstimationName = estimation.EstimationName,
            LinesOfCode = estimation.LinesOfCode,
            EstimatedEffort = estimation.EstimatedEffort,
            EstimatedCost = estimation.EstimatedCost,
            EstimatedTime = estimation.EstimatedTime,
            Notes = estimation.Notes,
            CreatedAt = estimation.CreatedAt,
            UpdatedAt = estimation.UpdatedAt
        };
    }
}
