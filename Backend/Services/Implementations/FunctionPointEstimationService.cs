using Backend.Models.DTOs;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

/// <summary>
/// Service implementation for Function Point Estimation operations
/// Function Points method: Estimates effort, time and cost based on functional components
/// </summary>
public class FunctionPointEstimationService : IFunctionPointEstimationService
{
    private readonly IFunctionPointEstimationRepository _repository;
    private readonly ILogger<FunctionPointEstimationService> _logger;

    // Complexity weights for different FP components
    private readonly Dictionary<string, (int low, int avg, int high)> _complexityWeights = new()
    {
        { "EI", (3, 4, 6) },     // External Inputs
        { "EO", (4, 5, 7) },     // External Outputs
        { "EQ", (3, 4, 6) },     // External Inquiries
        { "ILF", (7, 10, 15) },  // Internal Logical Files
        { "EIF", (5, 7, 10) }    // External Interface Files
    };

    // FP to effort conversion constants (can be moved to database/config)
    private const decimal EffortPerFp = 0.67m;       // Effort in person-months per FP
    private const decimal CostPerPersonMonth = 8000m; // Cost per person-month
    private const decimal TimeCoefficient = 0.38m;    // Time factor

    public FunctionPointEstimationService(
        IFunctionPointEstimationRepository repository,
        ILogger<FunctionPointEstimationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<FunctionPointEstimationDto>> GetEstimationsByProjectAsync(int projectId)
    {
        try
        {
            var estimations = await _repository.GetEstimationsByProjectAsync(projectId);
            return estimations.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Function Point estimations for project {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<FunctionPointEstimationDto?> GetEstimationByIdAsync(int estimationId)
    {
        try
        {
            var estimation = await _repository.GetEstimationByIdAsync(estimationId);
            return estimation != null ? MapToDto(estimation) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Function Point estimation {EstimationId}", estimationId);
            throw;
        }
    }

    public async Task<FunctionPointEstimationDto> CreateEstimationAsync(CreateFunctionPointEstimationDto createDto)
    {
        try
        {
            // Calculate unadjusted Function Points
            decimal unadjustedFp = CalculateUnadjustedFP(
                createDto.ExternalInputs,
                createDto.ExternalOutputs,
                createDto.ExternalInquiries,
                createDto.InternalLogicalFiles,
                createDto.ExternalInterfaceFiles,
                createDto.ComplexityLevel
            );

            // Calculate Value Adjustment Factor (default = 1.0 for now, can be extended with 14 characteristics)
            decimal valueAdjustmentFactor = 1.0m; // Standard adjustment

            // Calculate adjusted Function Points
            decimal adjustedFp = unadjustedFp * valueAdjustmentFactor;

            // Calculate effort (person-months)
            decimal estimatedEffort = CalculateEffort(adjustedFp);

            // Calculate estimated time (months)
            decimal estimatedTime = CalculateTime(estimatedEffort);

            // Calculate estimated cost
            decimal estimatedCost = CalculateCost(estimatedEffort);

            var estimation = new FunctionPointEstimation
            {
                ProjectId = createDto.ProjectId,
                EstimationName = createDto.EstimationName,
                ExternalInputs = createDto.ExternalInputs,
                ExternalOutputs = createDto.ExternalOutputs,
                ExternalInquiries = createDto.ExternalInquiries,
                InternalLogicalFiles = createDto.InternalLogicalFiles,
                ExternalInterfaceFiles = createDto.ExternalInterfaceFiles,
                ComplexityLevel = Enum.Parse<FPComplexityLevel>(createDto.ComplexityLevel),
                UnadjustedFp = Math.Round(unadjustedFp, 2),
                ValueAdjustmentFactor = Math.Round(valueAdjustmentFactor, 4),
                AdjustedFp = Math.Round(adjustedFp, 2),
                EstimatedEffort = Math.Round(estimatedEffort, 2),
                EstimatedTime = Math.Round(estimatedTime, 2),
                EstimatedCost = Math.Round(estimatedCost, 2),
                Notes = createDto.Notes
            };

            var createdEstimation = await _repository.CreateEstimationAsync(estimation);

            _logger.LogInformation("Function Point estimation created: {EstimationId} for project {ProjectId}",
                createdEstimation.FpEstimationId, createDto.ProjectId);

            return MapToDto(createdEstimation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Function Point estimation for project {ProjectId}", createDto.ProjectId);
            throw;
        }
    }

    public async Task<FunctionPointEstimationDto> UpdateEstimationAsync(UpdateFunctionPointEstimationDto updateDto)
    {
        try
        {
            var estimation = await _repository.GetEstimationByIdAsync(updateDto.FpEstimationId);
            if (estimation == null)
            {
                throw new KeyNotFoundException($"Function Point estimation with ID {updateDto.FpEstimationId} not found");
            }

            // Recalculate metrics based on new values
            decimal unadjustedFp = CalculateUnadjustedFP(
                updateDto.ExternalInputs,
                updateDto.ExternalOutputs,
                updateDto.ExternalInquiries,
                updateDto.InternalLogicalFiles,
                updateDto.ExternalInterfaceFiles,
                updateDto.ComplexityLevel
            );

            decimal valueAdjustmentFactor = 1.0m;
            decimal adjustedFp = unadjustedFp * valueAdjustmentFactor;
            decimal estimatedEffort = CalculateEffort(adjustedFp);
            decimal estimatedTime = CalculateTime(estimatedEffort);
            decimal estimatedCost = CalculateCost(estimatedEffort);

            estimation.EstimationName = updateDto.EstimationName;
            estimation.ExternalInputs = updateDto.ExternalInputs;
            estimation.ExternalOutputs = updateDto.ExternalOutputs;
            estimation.ExternalInquiries = updateDto.ExternalInquiries;
            estimation.InternalLogicalFiles = updateDto.InternalLogicalFiles;
            estimation.ExternalInterfaceFiles = updateDto.ExternalInterfaceFiles;
            estimation.ComplexityLevel = Enum.Parse<FPComplexityLevel>(updateDto.ComplexityLevel);
            estimation.UnadjustedFp = Math.Round(unadjustedFp, 2);
            estimation.ValueAdjustmentFactor = Math.Round(valueAdjustmentFactor, 4);
            estimation.AdjustedFp = Math.Round(adjustedFp, 2);
            estimation.EstimatedEffort = Math.Round(estimatedEffort, 2);
            estimation.EstimatedTime = Math.Round(estimatedTime, 2);
            estimation.EstimatedCost = Math.Round(estimatedCost, 2);
            estimation.Notes = updateDto.Notes;

            var updatedEstimation = await _repository.UpdateEstimationAsync(estimation);

            _logger.LogInformation("Function Point estimation updated: {EstimationId}", updateDto.FpEstimationId);

            return MapToDto(updatedEstimation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Function Point estimation {EstimationId}", updateDto.FpEstimationId);
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
                _logger.LogInformation("Function Point estimation deleted: {EstimationId}", estimationId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Function Point estimation {EstimationId}", estimationId);
            throw;
        }
    }

    /// <summary>
    /// Calculate unadjusted Function Points based on component counts and complexity
    /// Formula: FP = Σ(component_count * complexity_weight)
    /// </summary>
    private decimal CalculateUnadjustedFP(
        int externalInputs,
        int externalOutputs,
        int externalInquiries,
        int internalLogicalFiles,
        int externalInterfaceFiles,
        string complexityLevel)
    {
        var complexity = complexityLevel.ToUpper() switch
        {
            "LOW" => 0,
            "AVERAGE" => 1,
            "HIGH" => 2,
            _ => 1
        };

        decimal totalFp = 0;

        // External Inputs (EI)
        totalFp += externalInputs * GetWeight("EI", complexity);

        // External Outputs (EO)
        totalFp += externalOutputs * GetWeight("EO", complexity);

        // External Inquiries (EQ)
        totalFp += externalInquiries * GetWeight("EQ", complexity);

        // Internal Logical Files (ILF)
        totalFp += internalLogicalFiles * GetWeight("ILF", complexity);

        // External Interface Files (EIF)
        totalFp += externalInterfaceFiles * GetWeight("EIF", complexity);

        return totalFp;
    }

    /// <summary>
    /// Get complexity weight for a component based on complexity level
    /// 0=Low, 1=Average, 2=High
    /// </summary>
    private int GetWeight(string component, int complexityIndex)
    {
        var weights = _complexityWeights[component];
        return complexityIndex switch
        {
            0 => weights.low,
            1 => weights.avg,
            2 => weights.high,
            _ => weights.avg
        };
    }

    /// <summary>
    /// Calculate effort in person-months
    /// Formula: Effort = AdjustedFP * EffortPerFp
    /// </summary>
    private decimal CalculateEffort(decimal adjustedFp)
    {
        return adjustedFp * EffortPerFp;
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
    /// Map FunctionPointEstimation entity to DTO
    /// </summary>
    private FunctionPointEstimationDto MapToDto(FunctionPointEstimation estimation)
    {
        return new FunctionPointEstimationDto
        {
            FpEstimationId = estimation.FpEstimationId,
            ProjectId = estimation.ProjectId,
            EstimationName = estimation.EstimationName,
            ExternalInputs = estimation.ExternalInputs,
            ExternalOutputs = estimation.ExternalOutputs,
            ExternalInquiries = estimation.ExternalInquiries,
            InternalLogicalFiles = estimation.InternalLogicalFiles,
            ExternalInterfaceFiles = estimation.ExternalInterfaceFiles,
            ComplexityLevel = estimation.ComplexityLevel.ToString(),
            UnadjustedFp = estimation.UnadjustedFp,
            ValueAdjustmentFactor = estimation.ValueAdjustmentFactor,
            AdjustedFp = estimation.AdjustedFp,
            EstimatedEffort = estimation.EstimatedEffort,
            EstimatedCost = estimation.EstimatedCost,
            EstimatedTime = estimation.EstimatedTime,
            Notes = estimation.Notes,
            CreatedAt = estimation.CreatedAt,
            UpdatedAt = estimation.UpdatedAt,
            Characteristics = estimation.Characteristics?.Select(c => new FunctionPointCharacteristicDto
            {
                FpCharId = c.FpCharId,
                FpEstimationId = c.FpEstimationId,
                CharacteristicName = c.CharacteristicName,
                InfluenceLevel = c.InfluenceLevel,
                Score = c.Score,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList()
        };
    }
}
