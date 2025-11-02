using Backend.Models.DTOs;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

/// <summary>
/// Service implementation for Use Case Point Estimation operations
/// Use Case Points method: Estimates effort, time and cost based on use cases and actors
/// </summary>
public class UseCasePointEstimationService : IUseCasePointEstimationService
{
    private readonly IUseCasePointEstimationRepository _repository;
    private readonly ILogger<UseCasePointEstimationService> _logger;

    // UCP weights for complexity
    private readonly Dictionary<string, int> _ucComplexityWeights = new()
    {
        { "simple", 5 },
        { "average", 10 },
        { "complex", 15 }
    };

    private readonly Dictionary<string, int> _actorComplexityWeights = new()
    {
        { "simple", 1 },
        { "average", 2 },
        { "complex", 3 }
    };

    // Conversion constants
    private const decimal HoursPerUcp = 20m;                   // Hours per UCP
    private const decimal HoursPerPersonMonth = 160m;          // Working hours per month
    private const decimal CostPerPersonMonth = 8000m;          // Cost per person-month
    private const decimal TimeCoefficient = 0.38m;             // Time factor

    // Default factors (can be overridden with technical and environment factors)
    private const decimal DefaultTcf = 1.0m;                   // Technical Complexity Factor
    private const decimal DefaultEf = 1.0m;                    // Environment Factor

    public UseCasePointEstimationService(
        IUseCasePointEstimationRepository repository,
        ILogger<UseCasePointEstimationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<UseCasePointEstimationDto>> GetEstimationsByProjectAsync(int projectId)
    {
        try
        {
            var estimations = await _repository.GetEstimationsByProjectAsync(projectId);
            return estimations.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Use Case Point estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<UseCasePointEstimationDto?> GetEstimationByIdAsync(int estimationId)
    {
        try
        {
            var estimation = await _repository.GetEstimationByIdAsync(estimationId);
            return estimation != null ? MapToDto(estimation) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Use Case Point estimation {EstimationId}", estimationId);
            throw;
        }
    }

    public async Task<UseCasePointEstimationDto> CreateEstimationAsync(CreateUseCasePointEstimationDto createDto)
    {
        try
        {
            // Calculate unadjusted UCP
            decimal unadjustedUcp = CalculateUnadjustedUCP(
                createDto.SimpleUccCount,
                createDto.AverageUccCount,
                createDto.ComplexUccCount,
                createDto.SimpleActorCount,
                createDto.AverageActorCount,
                createDto.ComplexActorCount
            );

            // Use default factors (can be extended with database values)
            decimal tcf = DefaultTcf;
            decimal ef = DefaultEf;

            // Calculate adjusted UCP
            decimal adjustedUcp = unadjustedUcp * tcf * ef;

            // Calculate effort in hours
            decimal effortHours = adjustedUcp * HoursPerUcp;

            // Convert to person-months
            decimal effortPm = effortHours / HoursPerPersonMonth;

            // Calculate estimated time (months)
            decimal estimatedTime = CalculateTime(effortPm);

            // Calculate estimated cost
            decimal estimatedCost = CalculateCost(effortPm);

            var estimation = new UseCasePointEstimation
            {
                ProjectId = createDto.ProjectId,
                EstimationName = createDto.EstimationName,
                SimpleUccCount = createDto.SimpleUccCount,
                AverageUccCount = createDto.AverageUccCount,
                ComplexUccCount = createDto.ComplexUccCount,
                SimpleActorCount = createDto.SimpleActorCount,
                AverageActorCount = createDto.AverageActorCount,
                ComplexActorCount = createDto.ComplexActorCount,
                UnadjustedUcp = Math.Round(unadjustedUcp, 2),
                TechnicalComplexityFactor = Math.Round(tcf, 4),
                EnvironmentFactor = Math.Round(ef, 4),
                AdjustedUcp = Math.Round(adjustedUcp, 2),
                EstimatedEffort = Math.Round(effortHours, 2),
                EstimatedEffortPm = Math.Round(effortPm, 2),
                EstimatedTime = Math.Round(estimatedTime, 2),
                EstimatedCost = Math.Round(estimatedCost, 2),
                Notes = createDto.Notes
            };

            var createdEstimation = await _repository.CreateEstimationAsync(estimation);

            _logger.LogInformation("Use Case Point estimation created: {EstimationId} for project {ProjectId}",
                createdEstimation.UcpEstimationId, createDto.ProjectId);

            return MapToDto(createdEstimation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Use Case Point estimation for project {ProjectId}", createDto.ProjectId);
            throw;
        }
    }

    public async Task<UseCasePointEstimationDto> UpdateEstimationAsync(UpdateUseCasePointEstimationDto updateDto)
    {
        try
        {
            var estimation = await _repository.GetEstimationByIdAsync(updateDto.UcpEstimationId);
            if (estimation == null)
            {
                throw new KeyNotFoundException($"Use Case Point estimation with ID {updateDto.UcpEstimationId} not found");
            }

            // Recalculate metrics
            decimal unadjustedUcp = CalculateUnadjustedUCP(
                updateDto.SimpleUccCount,
                updateDto.AverageUccCount,
                updateDto.ComplexUccCount,
                updateDto.SimpleActorCount,
                updateDto.AverageActorCount,
                updateDto.ComplexActorCount
            );

            decimal tcf = DefaultTcf;
            decimal ef = DefaultEf;
            decimal adjustedUcp = unadjustedUcp * tcf * ef;
            decimal effortHours = adjustedUcp * HoursPerUcp;
            decimal effortPm = effortHours / HoursPerPersonMonth;
            decimal estimatedTime = CalculateTime(effortPm);
            decimal estimatedCost = CalculateCost(effortPm);

            estimation.EstimationName = updateDto.EstimationName;
            estimation.SimpleUccCount = updateDto.SimpleUccCount;
            estimation.AverageUccCount = updateDto.AverageUccCount;
            estimation.ComplexUccCount = updateDto.ComplexUccCount;
            estimation.SimpleActorCount = updateDto.SimpleActorCount;
            estimation.AverageActorCount = updateDto.AverageActorCount;
            estimation.ComplexActorCount = updateDto.ComplexActorCount;
            estimation.UnadjustedUcp = Math.Round(unadjustedUcp, 2);
            estimation.TechnicalComplexityFactor = Math.Round(tcf, 4);
            estimation.EnvironmentFactor = Math.Round(ef, 4);
            estimation.AdjustedUcp = Math.Round(adjustedUcp, 2);
            estimation.EstimatedEffort = Math.Round(effortHours, 2);
            estimation.EstimatedEffortPm = Math.Round(effortPm, 2);
            estimation.EstimatedTime = Math.Round(estimatedTime, 2);
            estimation.EstimatedCost = Math.Round(estimatedCost, 2);
            estimation.Notes = updateDto.Notes;

            var updatedEstimation = await _repository.UpdateEstimationAsync(estimation);

            _logger.LogInformation("Use Case Point estimation updated: {EstimationId}", updateDto.UcpEstimationId);

            return MapToDto(updatedEstimation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Use Case Point estimation {EstimationId}", updateDto.UcpEstimationId);
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
                _logger.LogInformation("Use Case Point estimation deleted: {EstimationId}", estimationId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Use Case Point estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Calculate unadjusted Use Case Points
    /// Formula: UUCP = (SimpleUC*5 + AverageUC*10 + ComplexUC*15) + (SimpleActor*1 + AverageActor*2 + ComplexActor*3)
    /// </summary>
    private decimal CalculateUnadjustedUCP(
        int simpleUcc,
        int averageUcc,
        int complexUcc,
        int simpleActor,
        int averageActor,
        int complexActor)
    {
        decimal ucWeight = (simpleUcc * _ucComplexityWeights["simple"]) +
                          (averageUcc * _ucComplexityWeights["average"]) +
                          (complexUcc * _ucComplexityWeights["complex"]);

        decimal actorWeight = (simpleActor * _actorComplexityWeights["simple"]) +
                             (averageActor * _actorComplexityWeights["average"]) +
                             (complexActor * _actorComplexityWeights["complex"]);

        return ucWeight + actorWeight;
    }

    /// <summary>
    /// Calculate estimated time in months
    /// Formula: Time = 2.5 * (Effort ^ 0.38)
    /// </summary>
    private decimal CalculateTime(decimal effortPm)
    {
        return 2.5m * (decimal)Math.Pow((double)effortPm, (double)TimeCoefficient);
    }

    /// <summary>
    /// Calculate estimated cost
    /// Formula: Cost = Effort (person-months) * CostPerPersonMonth
    /// </summary>
    private decimal CalculateCost(decimal effortPm)
    {
        return effortPm * CostPerPersonMonth;
    }

    /// <summary>
    /// Map UseCasePointEstimation entity to DTO
    /// </summary>
    private UseCasePointEstimationDto MapToDto(UseCasePointEstimation estimation)
    {
        return new UseCasePointEstimationDto
        {
            UcpEstimationId = estimation.UcpEstimationId,
            ProjectId = estimation.ProjectId,
            EstimationName = estimation.EstimationName,
            SimpleUccCount = estimation.SimpleUccCount,
            AverageUccCount = estimation.AverageUccCount,
            ComplexUccCount = estimation.ComplexUccCount,
            SimpleActorCount = estimation.SimpleActorCount,
            AverageActorCount = estimation.AverageActorCount,
            ComplexActorCount = estimation.ComplexActorCount,
            UnadjustedUcp = estimation.UnadjustedUcp,
            TechnicalComplexityFactor = estimation.TechnicalComplexityFactor,
            EnvironmentFactor = estimation.EnvironmentFactor,
            AdjustedUcp = estimation.AdjustedUcp,
            EstimatedEffort = estimation.EstimatedEffort,
            EstimatedEffortPm = estimation.EstimatedEffortPm,
            EstimatedCost = estimation.EstimatedCost,
            EstimatedTime = estimation.EstimatedTime,
            Notes = estimation.Notes,
            CreatedAt = estimation.CreatedAt,
            UpdatedAt = estimation.UpdatedAt,
            TechnicalFactors = estimation.TechnicalFactors?.Select(tf => new UseCaseTechnicalFactorDto
            {
                UcpTechFactorId = tf.UcpTechFactorId,
                UcpEstimationId = tf.UcpEstimationId,
                FactorName = tf.FactorName,
                FactorWeight = tf.FactorWeight,
                CreatedAt = tf.CreatedAt,
                UpdatedAt = tf.UpdatedAt
            }).ToList(),
            EnvironmentFactors = estimation.EnvironmentFactors?.Select(ef => new UseCaseEnvironmentFactorDto
            {
                UcpEnvFactorId = ef.UcpEnvFactorId,
                UcpEstimationId = ef.UcpEstimationId,
                FactorName = ef.FactorName,
                FactorWeight = ef.FactorWeight,
                CreatedAt = ef.CreatedAt,
                UpdatedAt = ef.UpdatedAt
            }).ToList()
        };
    }
}
