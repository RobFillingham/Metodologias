using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Backend.Services.Interfaces.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoThree;

namespace Backend.Services.Implementations.CocomoTwoStageOne;

/// <summary>
/// Service implementation for COCOMO 2 Stage 1 calculation operations
/// Contains all the business logic for calculating effort multipliers and COCOMO metrics
/// </summary>
public class CocomoCalculationCocomo2Stage1Service : ICocomoCalculationCocomo2Stage1Service
{
    private readonly IEstimationCocomo2Stage1Repository _estimationRepository;
    private readonly IComponentCocomo2Stage1Repository _componentRepository;
    private readonly IParameterSetCocomo2Stage1Repository _parameterSetRepository;
    private readonly ILanguageRepository _languageRepository;

    public CocomoCalculationCocomo2Stage1Service(
        IEstimationCocomo2Stage1Repository estimationRepository,
        IComponentCocomo2Stage1Repository componentRepository,
        IParameterSetCocomo2Stage1Repository parameterSetRepository,
        ILanguageRepository languageRepository)
    {
        _estimationRepository = estimationRepository;
        _componentRepository = componentRepository;
        _parameterSetRepository = parameterSetRepository;
        _languageRepository = languageRepository;
    }

    public decimal GetMultiplierValue(ParameterSetCocomo2Stage1 paramSet, string factor, string rating)
    {
        // Convert to PascalCase for property matching
        var factorPascal = ToPascalCase(factor);
        var ratingPascal = ToPascalCase(rating);
        
        // Special handling for ratings: "very_low" -> "VeryLow", "nominal" -> "Nominal"
        ratingPascal = rating.ToLower() switch
        {
            "very_low" => "VeryLow",
            "low" => "Low",
            "nominal" => "Nominal",
            "high" => "High",
            _ => ToPascalCase(rating)
        };

        var propertyName = $"{factorPascal}{ratingPascal}";
        var property = typeof(ParameterSetCocomo2Stage1).GetProperty(propertyName);

        if (property == null)
        {
            throw new ArgumentException($"Invalid factor/rating combination: {factor}/{rating} (looking for property: {propertyName})");
        }

        var value = property.GetValue(paramSet) as decimal?;
        if (!value.HasValue)
        {
            throw new InvalidOperationException($"Multiplier value not found for {factor}/{rating}");
        }

        return value.Value;
    }

    public decimal CalculateEAF(EstimationCocomo2Stage1 estimation, ParameterSetCocomo2Stage1 paramSet)
    {
        decimal eaf = 1.0m;

        // AEXP: Application Experience
        var aexpValue = GetMultiplierValue(paramSet, "Aexp", estimation.SelectedAexp);
        eaf *= aexpValue;

        // PEXP: Platform Experience
        var pexpValue = GetMultiplierValue(paramSet, "Pexp", estimation.SelectedPexp);
        eaf *= pexpValue;

        // PREC: Precedentedness
        var precValue = GetMultiplierValue(paramSet, "Prec", estimation.SelectedPrec);
        eaf *= precValue;

        // RELY: Required Reliability
        var relyValue = GetMultiplierValue(paramSet, "Rely", estimation.SelectedRely);
        eaf *= relyValue;

        // TMSP: Time to Market Pressure
        var tmspValue = GetMultiplierValue(paramSet, "Tmsp", estimation.SelectedTmsp);
        eaf *= tmspValue;

        return eaf;
    }

    public (decimal sloc, decimal ksloc) ConvertToSLOC(decimal totalFp, decimal slocPerFp)
    {
        var sloc = totalFp * slocPerFp;
        var ksloc = sloc / 1000m;
        return (sloc, ksloc);
    }

    public decimal CalculateEffort(decimal constA, decimal constB, decimal ksloc, decimal eaf)
    {
        // PM = A × (KSLOC)^B × EAF
        if (ksloc == 0)
            return 0;

        return constA * (decimal)Math.Pow((double)ksloc, (double)constB) * eaf;
    }

    public decimal CalculateEffortHours(decimal effortPm)
    {
        // Convert Person-Months to Hours (152 hours per person-month)
        return effortPm * 152m;
    }

    public async Task RecalculateEstimationAsync(int estimationId)
    {
        // 1. Get estimation with all related data
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        // 2. Calculate total FP from components (considering reuse and changes)
        var totalFp = await _componentRepository.GetTotalFpAsync(estimationId);
        estimation.TotalFp = totalFp;

        // 3. Convert to SLOC using language factor
        if (estimation.Language == null)
        {
            throw new InvalidOperationException("Language information is missing");
        }
        var (sloc, ksloc) = ConvertToSLOC(totalFp, estimation.Language.SlocPerUfp);
        estimation.Sloc = sloc;
        estimation.Ksloc = ksloc;

        // 4. Get parameter set
        if (estimation.ParameterSet == null)
        {
            throw new InvalidOperationException("Parameter set information is missing");
        }

        // 5. Calculate individual multipliers and store them
        estimation.AexpMultiplier = GetMultiplierValue(estimation.ParameterSet, "Aexp", estimation.SelectedAexp);
        estimation.PexpMultiplier = GetMultiplierValue(estimation.ParameterSet, "Pexp", estimation.SelectedPexp);
        estimation.PrecMultiplier = GetMultiplierValue(estimation.ParameterSet, "Prec", estimation.SelectedPrec);
        estimation.RelyMultiplier = GetMultiplierValue(estimation.ParameterSet, "Rely", estimation.SelectedRely);
        estimation.TmspMultiplier = GetMultiplierValue(estimation.ParameterSet, "Tmsp", estimation.SelectedTmsp);

        // 6. Calculate EAF (product of all multipliers)
        var eaf = CalculateEAF(estimation, estimation.ParameterSet);
        estimation.Eaf = eaf;

        // 7. Calculate Effort (PM)
        var effortPm = CalculateEffort(
            estimation.ParameterSet.ConstA,
            estimation.ParameterSet.ConstB,
            ksloc,
            eaf
        );
        estimation.EffortPm = effortPm;

        // 8. Calculate Effort in Hours
        var effortHours = CalculateEffortHours(effortPm);
        estimation.EffortHours = effortHours;

        // 9. Save updated estimation
        await _estimationRepository.UpdateAsync(estimation);
    }

    #region Private Helper Methods

    /// <summary>
    /// Convert a string to PascalCase (first letter uppercase, rest lowercase)
    /// </summary>
    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    #endregion
}
