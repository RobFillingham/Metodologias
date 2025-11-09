using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Services.Implementations.CocomoThree;

/// <summary>
/// Service implementation for COCOMO II calculation operations
/// Contains all the business logic for calculating complexity, function points, and COCOMO metrics
/// </summary>
public class CocomoCalculationService : ICocomoCalculationService
{
    private readonly IEstimationRepository _estimationRepository;
    private readonly IEstimationFunctionRepository _functionRepository;
    private readonly IParameterSetRepository _parameterSetRepository;
    private readonly ILanguageRepository _languageRepository;

    public CocomoCalculationService(
        IEstimationRepository estimationRepository,
        IEstimationFunctionRepository functionRepository,
        IParameterSetRepository parameterSetRepository,
        ILanguageRepository languageRepository)
    {
        _estimationRepository = estimationRepository;
        _functionRepository = functionRepository;
        _parameterSetRepository = parameterSetRepository;
        _languageRepository = languageRepository;
    }

    public string DetermineComplexity(string type, int det, int retFtr)
    {
        return type.ToUpper() switch
        {
            "EI" => DetermineEIComplexity(det, retFtr),
            "EO" => DetermineEOComplexity(det, retFtr),
            "EQ" => DetermineEQComplexity(det, retFtr),
            "ILF" => DetermineILFComplexity(det, retFtr),
            "EIF" => DetermineEIFComplexity(det, retFtr),
            _ => throw new ArgumentException($"Invalid function type: {type}")
        };
    }

    public decimal GetFunctionPoints(string type, string complexity)
    {
        return type.ToUpper() switch
        {
            "EI" => complexity switch
            {
                "Baja" => 3,
                "Media" => 4,
                "Alta" => 6,
                _ => throw new ArgumentException($"Invalid complexity: {complexity}")
            },
            "EO" => complexity switch
            {
                "Baja" => 4,
                "Media" => 5,
                "Alta" => 7,
                _ => throw new ArgumentException($"Invalid complexity: {complexity}")
            },
            "EQ" => complexity switch
            {
                "Baja" => 3,
                "Media" => 4,
                "Alta" => 6,
                _ => throw new ArgumentException($"Invalid complexity: {complexity}")
            },
            "ILF" => complexity switch
            {
                "Baja" => 7,
                "Media" => 10,
                "Alta" => 15,
                _ => throw new ArgumentException($"Invalid complexity: {complexity}")
            },
            "EIF" => complexity switch
            {
                "Baja" => 5,
                "Media" => 7,
                "Alta" => 10,
                _ => throw new ArgumentException($"Invalid complexity: {complexity}")
            },
            _ => throw new ArgumentException($"Invalid function type: {type}")
        };
    }

    public async Task CalculateFunctionComplexityAsync(EstimationFunction function)
    {
        if (!function.Det.HasValue || !function.RetFtr.HasValue)
        {
            throw new ArgumentException("DET and RET/FTR values are required");
        }

        // Determine complexity
        function.Complexity = DetermineComplexity(function.Type, function.Det.Value, function.RetFtr.Value);

        // Assign function points
        function.CalculatedPoints = GetFunctionPoints(function.Type, function.Complexity);

        await Task.CompletedTask;
    }

    public async Task<decimal> CalculateTotalUFPAsync(int estimationId)
    {
        return await _functionRepository.GetTotalUfpAsync(estimationId);
    }

    public (decimal sloc, decimal ksloc) ConvertToSLOC(decimal totalUfp, decimal slocPerUfp)
    {
        var sloc = totalUfp * slocPerUfp;
        var ksloc = sloc / 1000m;
        return (sloc, ksloc);
    }

    public decimal CalculateSumSF(Estimation estimation, ParameterSet parameterSet)
    {
        decimal sumSf = 0;

        // PREC
        sumSf += GetSFValue(parameterSet, "PREC", estimation.SelectedSfPrec);

        // FLEX
        sumSf += GetSFValue(parameterSet, "FLEX", estimation.SelectedSfFlex);

        // RESL
        sumSf += GetSFValue(parameterSet, "RESL", estimation.SelectedSfResl);

        // TEAM
        sumSf += GetSFValue(parameterSet, "TEAM", estimation.SelectedSfTeam);

        // PMAT
        sumSf += GetSFValue(parameterSet, "PMAT", estimation.SelectedSfPmat);

        return sumSf;
    }

    public decimal CalculateExponent(decimal constB, decimal sumSf)
    {
        return constB + (0.01m * sumSf);
    }

    public decimal CalculateEAF(Estimation estimation, ParameterSet parameterSet)
    {
        decimal eaf = 1.0m;

        // PERS
        eaf *= GetEMValue(parameterSet, "PERS", estimation.SelectedEmPers);

        // RCPX
        eaf *= GetEMValue(parameterSet, "RCPX", estimation.SelectedEmRcpx);

        // PDIF
        eaf *= GetEMValue(parameterSet, "PDIF", estimation.SelectedEmPdif);

        // PREX
        eaf *= GetEMValue(parameterSet, "PREX", estimation.SelectedEmPrex);

        // RUSE
        eaf *= GetEMValue(parameterSet, "RUSE", estimation.SelectedEmRuse);

        // FCIL
        eaf *= GetEMValue(parameterSet, "FCIL", estimation.SelectedEmFcil);

        // SCED
        eaf *= GetEMValue(parameterSet, "SCED", estimation.SelectedEmSced);

        return eaf;
    }

    public decimal CalculateEffort(decimal constA, decimal ksloc, decimal exponentE, decimal eaf)
    {
        // PM = A × (KSLOC ^ E) × EAF
        return constA * (decimal)Math.Pow((double)ksloc, (double)exponentE) * eaf;
    }

    public decimal CalculateDuration(decimal constC, decimal constD, decimal constB, decimal effortPm, decimal exponentE)
    {
        // F = D + 0.2 × (E - B)
        var f = constD + (0.2m * (exponentE - constB));

        // TDEV = C × (PM ^ F)
        return constC * (decimal)Math.Pow((double)effortPm, (double)f);
    }

    public decimal CalculateTeamSize(decimal effortPm, decimal tdevMonths)
    {
        if (tdevMonths == 0)
            return 0;

        return effortPm / tdevMonths;
    }

    public async Task RecalculateEstimationAsync(int estimationId)
    {
        // 1. Get estimation with all related data
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        // 2. Calculate total UFP
        var totalUfp = await CalculateTotalUFPAsync(estimationId);
        estimation.TotalUfp = totalUfp;

        // 3. Convert to SLOC using language factor
        if (estimation.Language == null)
        {
            throw new InvalidOperationException("Language information is missing");
        }
        var (sloc, ksloc) = ConvertToSLOC(totalUfp, estimation.Language.SlocPerUfp);
        estimation.Sloc = sloc;
        estimation.Ksloc = ksloc;

        // 4. Get parameter set
        if (estimation.ParameterSet == null)
        {
            throw new InvalidOperationException("Parameter set information is missing");
        }

        // 5. Calculate Sum SF
        var sumSf = CalculateSumSF(estimation, estimation.ParameterSet);
        estimation.SumSf = sumSf;

        // 6. Calculate Exponent E
        var exponentE = CalculateExponent(estimation.ParameterSet.ConstB, sumSf);
        estimation.ExponentE = exponentE;

        // 7. Calculate EAF
        var eaf = CalculateEAF(estimation, estimation.ParameterSet);
        estimation.Eaf = eaf;

        // 8. Calculate Effort (PM)
        var effortPm = CalculateEffort(estimation.ParameterSet.ConstA, ksloc, exponentE, eaf);
        estimation.EffortPm = effortPm;

        // 9. Calculate Duration (TDEV)
        var tdevMonths = CalculateDuration(
            estimation.ParameterSet.ConstC,
            estimation.ParameterSet.ConstD,
            estimation.ParameterSet.ConstB,
            effortPm,
            exponentE
        );
        estimation.TdevMonths = tdevMonths;

        // 10. Calculate Team Size
        var avgTeamSize = CalculateTeamSize(effortPm, tdevMonths);
        estimation.AvgTeamSize = avgTeamSize;

        // 11. Save updated estimation
        await _estimationRepository.UpdateAsync(estimation);
    }

    #region Private Helper Methods

    private string DetermineEIComplexity(int det, int ftr)
    {
        // Baja: DET ≤ 4 Y FTR ≤ 1
        if (det <= 4 && ftr <= 1)
            return "Baja";

        // Alta: DET ≥ 16 O FTR ≥ 4
        if (det >= 16 || ftr >= 4)
            return "Alta";

        // Media: (DET 5-15) O (FTR 2-3)
        return "Media";
    }

    private string DetermineEOComplexity(int det, int ftr)
    {
        // Baja: DET ≤ 5 Y FTR ≤ 1
        if (det <= 5 && ftr <= 1)
            return "Baja";

        // Alta: DET ≥ 20 O FTR ≥ 4
        if (det >= 20 || ftr >= 4)
            return "Alta";

        // Media: (DET 6-19) O (FTR 2-3)
        return "Media";
    }

    private string DetermineEQComplexity(int det, int ftr)
    {
        // Baja: DET ≤ 5 Y FTR ≤ 1
        if (det <= 5 && ftr <= 1)
            return "Baja";

        // Alta: DET ≥ 20 O FTR ≥ 4
        if (det >= 20 || ftr >= 4)
            return "Alta";

        // Media: (DET 6-19) O (FTR 2-3)
        return "Media";
    }

    private string DetermineILFComplexity(int det, int ret)
    {
        // Baja: DET ≤ 19 Y RET = 1
        if (det <= 19 && ret == 1)
            return "Baja";

        // Alta: DET ≥ 51 O RET ≥ 6
        if (det >= 51 || ret >= 6)
            return "Alta";

        // Media: (DET 20-50) O (RET 2-5)
        return "Media";
    }

    private string DetermineEIFComplexity(int det, int ret)
    {
        // Baja: DET ≤ 19 Y RET = 1
        if (det <= 19 && ret == 1)
            return "Baja";

        // Alta: DET ≥ 51 O RET ≥ 6
        if (det >= 51 || ret >= 6)
            return "Alta";

        // Media: (DET 20-50) O (RET 2-5)
        return "Media";
    }

    private decimal GetSFValue(ParameterSet paramSet, string factor, string rating)
    {
        // Convert to PascalCase: "PREC" -> "Prec", "NOM" -> "Nom"
        var factorPascal = ToPascalCase(factor);
        var ratingPascal = ToPascalCase(rating);
        var propertyName = $"Sf{factorPascal}{ratingPascal}";
        var property = typeof(ParameterSet).GetProperty(propertyName);
        
        if (property == null)
        {
            throw new ArgumentException($"Invalid SF factor/rating combination: {factor}/{rating} (looking for property: {propertyName})");
        }

        var value = property.GetValue(paramSet) as decimal?;
        if (!value.HasValue)
        {
            // For default parameter sets, this should not happen. For user-created sets, provide helpful error.
            if (paramSet.IsDefault)
            {
                throw new InvalidOperationException($"System error: Default parameter set '{paramSet.SetName}' is missing SF value for {factor}/{rating}. Please contact system administrator.");
            }
            else
            {
                throw new InvalidOperationException($"Parameter set '{paramSet.SetName}' (ID: {paramSet.ParamSetId}) is incomplete. All Scale Factor values must be configured. Please edit this parameter set to provide all required values.");
            }
        }

        return value.Value;
    }

    private decimal GetEMValue(ParameterSet paramSet, string factor, string rating)
    {
        // Convert to PascalCase: "PERS" -> "Pers", "NOM" -> "Nom"
        var factorPascal = ToPascalCase(factor);
        var ratingPascal = ToPascalCase(rating);
        var propertyName = $"Em{factorPascal}{ratingPascal}";
        var property = typeof(ParameterSet).GetProperty(propertyName);
        
        if (property == null)
        {
            throw new ArgumentException($"Invalid EM factor/rating combination: {factor}/{rating} (looking for property: {propertyName})");
        }

        var value = property.GetValue(paramSet) as decimal?;
        if (!value.HasValue)
        {
            throw new InvalidOperationException($"EM value not found for {factor}/{rating}");
        }

        return value.Value;
    }

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
