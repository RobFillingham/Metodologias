using Backend.Models.Entities.CocomoTwoStageOne;

namespace Backend.Services.Interfaces.CocomoTwoStageOne;

/// <summary>
/// Interface for COCOMO 2 Stage 1 calculation operations
/// Contains all the business logic for calculating effort multipliers and COCOMO metrics
/// </summary>
public interface ICocomoCalculationCocomo2Stage1Service
{
    /// <summary>
    /// Get the multiplier value for a specific factor and rating
    /// </summary>
    decimal GetMultiplierValue(ParameterSetCocomo2Stage1 paramSet, string factor, string rating);

    /// <summary>
    /// Calculate EAF (Effort Adjustment Factor) - product of all multipliers
    /// </summary>
    decimal CalculateEAF(EstimationCocomo2Stage1 estimation, ParameterSetCocomo2Stage1 paramSet);

    /// <summary>
    /// Convert Function Points to SLOC
    /// </summary>
    (decimal sloc, decimal ksloc) ConvertToSLOC(decimal totalFp, decimal slocPerFp);

    /// <summary>
    /// Calculate effort in Person-Months
    /// Formula: PM = A × (KSLOC)^B × EAF
    /// </summary>
    decimal CalculateEffort(decimal constA, decimal constB, decimal ksloc, decimal eaf);

    /// <summary>
    /// Calculate effort in hours
    /// Formula: Hours = PM × 152 (assuming 152 hours per person-month)
    /// </summary>
    decimal CalculateEffortHours(decimal effortPm);

    /// <summary>
    /// Recalculate entire estimation
    /// Called automatically when components or ratings change
    /// </summary>
    Task RecalculateEstimationAsync(int estimationId);
}
