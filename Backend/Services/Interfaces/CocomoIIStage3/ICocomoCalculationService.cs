using Backend.Models.Entities.CocomoIIStage3;

namespace Backend.Services.Interfaces.CocomoIIStage3;

/// <summary>
/// Service interface for COCOMO II Stage 3 calculation operations
/// </summary>
public interface ICocomoCalculationService
{
    /// <summary>
    /// Determine complexity level based on function type, DET, and RET/FTR
    /// </summary>
    /// <param name="type">Function type (EI, EO, EQ, ILF, EIF)</param>
    /// <param name="det">Data Element Types</param>
    /// <param name="retFtr">Record Element Types / File Types Referenced</param>
    /// <returns>Complexity level (Baja, Media, Alta)</returns>
    string DetermineComplexity(string type, int det, int retFtr);

    /// <summary>
    /// Get function points based on type and complexity
    /// </summary>
    /// <param name="type">Function type (EI, EO, EQ, ILF, EIF)</param>
    /// <param name="complexity">Complexity level (Baja, Media, Alta)</param>
    /// <returns>Function points</returns>
    decimal GetFunctionPoints(string type, string complexity);

    /// <summary>
    /// Calculate complexity and points for a function
    /// </summary>
    /// <param name="function">The estimation function to calculate</param>
    Task CalculateFunctionComplexityAsync(EstimationFunction function);

    /// <summary>
    /// Calculate total UFP for an estimation
    /// </summary>
    /// <param name="estimationId">The estimation ID</param>
    /// <returns>Total UFP</returns>
    Task<decimal> CalculateTotalUFPAsync(int estimationId);

    /// <summary>
    /// Convert UFP to SLOC using language factor
    /// </summary>
    /// <param name="totalUfp">Total Unadjusted Function Points</param>
    /// <param name="slocPerUfp">SLOC per UFP factor from language</param>
    /// <returns>SLOC and KSLOC</returns>
    (decimal sloc, decimal ksloc) ConvertToSLOC(decimal totalUfp, decimal slocPerUfp);

    /// <summary>
    /// Calculate sum of Scale Factors (SF)
    /// </summary>
    /// <param name="estimation">The estimation with selected ratings</param>
    /// <param name="parameterSet">The parameter set with SF values</param>
    /// <returns>Sum of SF values</returns>
    decimal CalculateSumSF(Estimation estimation, ParameterSet parameterSet);

    /// <summary>
    /// Calculate exponent E
    /// </summary>
    /// <param name="constB">Constant B from parameter set</param>
    /// <param name="sumSf">Sum of Scale Factors</param>
    /// <returns>Exponent E</returns>
    decimal CalculateExponent(decimal constB, decimal sumSf);

    /// <summary>
    /// Calculate Effort Adjustment Factor (EAF) - Updated for 17 EM factors
    /// </summary>
    /// <param name="estimation">The estimation with selected EM ratings</param>
    /// <param name="parameterSet">The parameter set with EM values</param>
    /// <returns>EAF value</returns>
    decimal CalculateEAF(Estimation estimation, ParameterSet parameterSet);

    /// <summary>
    /// Calculate effort in Person-Months (PM)
    /// </summary>
    /// <param name="constA">Constant A from parameter set</param>
    /// <param name="ksloc">Size in KSLOC</param>
    /// <param name="exponentE">Exponent E</param>
    /// <param name="eaf">Effort Adjustment Factor</param>
    /// <returns>Effort in Person-Months</returns>
    decimal CalculateEffort(decimal constA, decimal ksloc, decimal exponentE, decimal eaf);

    /// <summary>
    /// Calculate development duration in months (TDEV)
    /// </summary>
    /// <param name="constC">Constant C from parameter set</param>
    /// <param name="constD">Constant D from parameter set</param>
    /// <param name="constB">Constant B from parameter set</param>
    /// <param name="effortPm">Effort in Person-Months</param>
    /// <param name="exponentE">Exponent E</param>
    /// <returns>Development time in months</returns>
    decimal CalculateDuration(decimal constC, decimal constD, decimal constB, decimal effortPm, decimal exponentE);

    /// <summary>
    /// Calculate average team size
    /// </summary>
    /// <param name="effortPm">Effort in Person-Months</param>
    /// <param name="tdevMonths">Development time in months</param>
    /// <returns>Average team size</returns>
    decimal CalculateTeamSize(decimal effortPm, decimal tdevMonths);

    /// <summary>
    /// Recalculate entire estimation (called after adding/updating/deleting functions or changing ratings)
    /// This is the main orchestration method
    /// </summary>
    /// <param name="estimationId">The estimation ID to recalculate</param>
    Task RecalculateEstimationAsync(int estimationId);
}