using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Services.Implementations.CocomoThree;

/// <summary>
/// Implementation of ParameterSet service
/// </summary>
public class ParameterSetService : IParameterSetService
{
    private readonly IParameterSetRepository _parameterSetRepository;
    private readonly IEstimationRepository _estimationRepository;
    private readonly ILogger<ParameterSetService> _logger;

    public ParameterSetService(
        IParameterSetRepository parameterSetRepository,
        IEstimationRepository estimationRepository,
        ILogger<ParameterSetService> logger)
    {
        _parameterSetRepository = parameterSetRepository;
        _estimationRepository = estimationRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ParameterSetDto>> GetUserParameterSetsAsync(int userId)
    {
        var parameterSets = await _parameterSetRepository.GetByUserIdAsync(userId);
        return parameterSets.Select(MapToDto);
    }

    public async Task<IEnumerable<ParameterSetDto>> GetDefaultParameterSetsAsync()
    {
        var parameterSets = await _parameterSetRepository.GetDefaultParameterSetsAsync();
        return parameterSets.Select(MapToDto);
    }

    public async Task<ParameterSetDto?> GetParameterSetByIdAsync(int paramSetId, int userId)
    {
        var parameterSet = await _parameterSetRepository.GetByIdAsync(paramSetId);

        // Check if parameter set exists and user has access
        if (parameterSet == null)
            return null;

        // Allow access if it's a default set or belongs to the user
        if (parameterSet.IsDefault || parameterSet.UserId == userId)
            return MapToDto(parameterSet);

        return null;
    }

    public async Task<ParameterSetDto> CreateParameterSetAsync(CreateParameterSetDto createDto, int userId)
    {
        // Validate that set name is not empty
        if (string.IsNullOrWhiteSpace(createDto.SetName))
            throw new ArgumentException("Set name cannot be empty");

        // Check if user already has a parameter set with this name
        var existingSets = await _parameterSetRepository.GetByUserIdAsync(userId);
        if (existingSets.Any(ps => ps.SetName.Equals(createDto.SetName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A parameter set with this name already exists");

        // Validate that all required SF values are provided
        ValidateSFValues(createDto);

        var parameterSet = new ParameterSet
        {
            UserId = userId,
            SetName = createDto.SetName,
            IsDefault = false, // User-created sets are never default
            ConstA = createDto.ConstA,
            ConstB = createDto.ConstB,
            ConstC = createDto.ConstC,
            ConstD = createDto.ConstD,

            // Scale Factors
            SfPrecVlo = createDto.SfPrecVlo,
            SfPrecLo = createDto.SfPrecLo,
            SfPrecNom = createDto.SfPrecNom,
            SfPrecHi = createDto.SfPrecHi,
            SfPrecVhi = createDto.SfPrecVhi,
            SfPrecXhi = createDto.SfPrecXhi,

            SfFlexVlo = createDto.SfFlexVlo,
            SfFlexLo = createDto.SfFlexLo,
            SfFlexNom = createDto.SfFlexNom,
            SfFlexHi = createDto.SfFlexHi,
            SfFlexVhi = createDto.SfFlexVhi,
            SfFlexXhi = createDto.SfFlexXhi,

            SfReslVlo = createDto.SfReslVlo,
            SfReslLo = createDto.SfReslLo,
            SfReslNom = createDto.SfReslNom,
            SfReslHi = createDto.SfReslHi,
            SfReslVhi = createDto.SfReslVhi,
            SfReslXhi = createDto.SfReslXhi,

            SfTeamVlo = createDto.SfTeamVlo,
            SfTeamLo = createDto.SfTeamLo,
            SfTeamNom = createDto.SfTeamNom,
            SfTeamHi = createDto.SfTeamHi,
            SfTeamVhi = createDto.SfTeamVhi,
            SfTeamXhi = createDto.SfTeamXhi,

            SfPmatVlo = createDto.SfPmatVlo,
            SfPmatLo = createDto.SfPmatLo,
            SfPmatNom = createDto.SfPmatNom,
            SfPmatHi = createDto.SfPmatHi,
            SfPmatVhi = createDto.SfPmatVhi,
            SfPmatXhi = createDto.SfPmatXhi,

            // Effort Multipliers
            EmPersXlo = createDto.EmPersXlo,
            EmPersVlo = createDto.EmPersVlo,
            EmPersLo = createDto.EmPersLo,
            EmPersNom = createDto.EmPersNom,
            EmPersHi = createDto.EmPersHi,
            EmPersVhi = createDto.EmPersVhi,
            EmPersXhi = createDto.EmPersXhi,

            EmRcpxXlo = createDto.EmRcpxXlo,
            EmRcpxVlo = createDto.EmRcpxVlo,
            EmRcpxLo = createDto.EmRcpxLo,
            EmRcpxNom = createDto.EmRcpxNom,
            EmRcpxHi = createDto.EmRcpxHi,
            EmRcpxVhi = createDto.EmRcpxVhi,
            EmRcpxXhi = createDto.EmRcpxXhi,

            EmPdifXlo = createDto.EmPdifXlo,
            EmPdifVlo = createDto.EmPdifVlo,
            EmPdifLo = createDto.EmPdifLo,
            EmPdifNom = createDto.EmPdifNom,
            EmPdifHi = createDto.EmPdifHi,
            EmPdifVhi = createDto.EmPdifVhi,
            EmPdifXhi = createDto.EmPdifXhi,

            EmPrexXlo = createDto.EmPrexXlo,
            EmPrexVlo = createDto.EmPrexVlo,
            EmPrexLo = createDto.EmPrexLo,
            EmPrexNom = createDto.EmPrexNom,
            EmPrexHi = createDto.EmPrexHi,
            EmPrexVhi = createDto.EmPrexVhi,
            EmPrexXhi = createDto.EmPrexXhi,

            EmRuseXlo = createDto.EmRuseXlo,
            EmRuseVlo = createDto.EmRuseVlo,
            EmRuseLo = createDto.EmRuseLo,
            EmRuseNom = createDto.EmRuseNom,
            EmRuseHi = createDto.EmRuseHi,
            EmRuseVhi = createDto.EmRuseVhi,
            EmRuseXhi = createDto.EmRuseXhi,

            EmFcilXlo = createDto.EmFcilXlo,
            EmFcilVlo = createDto.EmFcilVlo,
            EmFcilLo = createDto.EmFcilLo,
            EmFcilNom = createDto.EmFcilNom,
            EmFcilHi = createDto.EmFcilHi,
            EmFcilVhi = createDto.EmFcilVhi,
            EmFcilXhi = createDto.EmFcilXhi,

            EmScedXlo = createDto.EmScedXlo,
            EmScedVlo = createDto.EmScedVlo,
            EmScedLo = createDto.EmScedLo,
            EmScedNom = createDto.EmScedNom,
            EmScedHi = createDto.EmScedHi,
            EmScedVhi = createDto.EmScedVhi,
            EmScedXhi = createDto.EmScedXhi
        };

        var createdParameterSet = await _parameterSetRepository.CreateAsync(parameterSet);
        _logger.LogInformation("Parameter set created: {SetName} for user {UserId}", createdParameterSet.SetName, userId);

        return MapToDto(createdParameterSet);
    }

    public async Task<ParameterSetDto> UpdateParameterSetAsync(UpdateParameterSetDto updateDto, int userId)
    {
        // Validate that set name is not empty
        if (string.IsNullOrWhiteSpace(updateDto.SetName))
            throw new ArgumentException("Set name cannot be empty");

        var existingParameterSet = await _parameterSetRepository.GetByIdAsync(updateDto.ParamSetId);

        if (existingParameterSet == null)
            throw new KeyNotFoundException("Parameter set not found");

        // Check ownership (users can only update their own sets, not default ones)
        if (existingParameterSet.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own parameter sets");

        // Check if another parameter set with this name already exists for this user
        var userSets = await _parameterSetRepository.GetByUserIdAsync(userId);
        if (userSets.Any(ps => ps.ParamSetId != updateDto.ParamSetId &&
                              ps.SetName.Equals(updateDto.SetName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A parameter set with this name already exists");

        // Validate that all required SF values are provided
        ValidateSFValuesForUpdate(updateDto);

        // Update the parameter set
        existingParameterSet.SetName = updateDto.SetName;
        existingParameterSet.ConstA = updateDto.ConstA;
        existingParameterSet.ConstB = updateDto.ConstB;
        existingParameterSet.ConstC = updateDto.ConstC;
        existingParameterSet.ConstD = updateDto.ConstD;

        // Scale Factors
        existingParameterSet.SfPrecVlo = updateDto.SfPrecVlo;
        existingParameterSet.SfPrecLo = updateDto.SfPrecLo;
        existingParameterSet.SfPrecNom = updateDto.SfPrecNom;
        existingParameterSet.SfPrecHi = updateDto.SfPrecHi;
        existingParameterSet.SfPrecVhi = updateDto.SfPrecVhi;
        existingParameterSet.SfPrecXhi = updateDto.SfPrecXhi;

        existingParameterSet.SfFlexVlo = updateDto.SfFlexVlo;
        existingParameterSet.SfFlexLo = updateDto.SfFlexLo;
        existingParameterSet.SfFlexNom = updateDto.SfFlexNom;
        existingParameterSet.SfFlexHi = updateDto.SfFlexHi;
        existingParameterSet.SfFlexVhi = updateDto.SfFlexVhi;
        existingParameterSet.SfFlexXhi = updateDto.SfFlexXhi;

        existingParameterSet.SfReslVlo = updateDto.SfReslVlo;
        existingParameterSet.SfReslLo = updateDto.SfReslLo;
        existingParameterSet.SfReslNom = updateDto.SfReslNom;
        existingParameterSet.SfReslHi = updateDto.SfReslHi;
        existingParameterSet.SfReslVhi = updateDto.SfReslVhi;
        existingParameterSet.SfReslXhi = updateDto.SfReslXhi;

        existingParameterSet.SfTeamVlo = updateDto.SfTeamVlo;
        existingParameterSet.SfTeamLo = updateDto.SfTeamLo;
        existingParameterSet.SfTeamNom = updateDto.SfTeamNom;
        existingParameterSet.SfTeamHi = updateDto.SfTeamHi;
        existingParameterSet.SfTeamVhi = updateDto.SfTeamVhi;
        existingParameterSet.SfTeamXhi = updateDto.SfTeamXhi;

        existingParameterSet.SfPmatVlo = updateDto.SfPmatVlo;
        existingParameterSet.SfPmatLo = updateDto.SfPmatLo;
        existingParameterSet.SfPmatNom = updateDto.SfPmatNom;
        existingParameterSet.SfPmatHi = updateDto.SfPmatHi;
        existingParameterSet.SfPmatVhi = updateDto.SfPmatVhi;
        existingParameterSet.SfPmatXhi = updateDto.SfPmatXhi;

        // Effort Multipliers
        existingParameterSet.EmPersXlo = updateDto.EmPersXlo;
        existingParameterSet.EmPersVlo = updateDto.EmPersVlo;
        existingParameterSet.EmPersLo = updateDto.EmPersLo;
        existingParameterSet.EmPersNom = updateDto.EmPersNom;
        existingParameterSet.EmPersHi = updateDto.EmPersHi;
        existingParameterSet.EmPersVhi = updateDto.EmPersVhi;
        existingParameterSet.EmPersXhi = updateDto.EmPersXhi;

        existingParameterSet.EmRcpxXlo = updateDto.EmRcpxXlo;
        existingParameterSet.EmRcpxVlo = updateDto.EmRcpxVlo;
        existingParameterSet.EmRcpxLo = updateDto.EmRcpxLo;
        existingParameterSet.EmRcpxNom = updateDto.EmRcpxNom;
        existingParameterSet.EmRcpxHi = updateDto.EmRcpxHi;
        existingParameterSet.EmRcpxVhi = updateDto.EmRcpxVhi;
        existingParameterSet.EmRcpxXhi = updateDto.EmRcpxXhi;

        existingParameterSet.EmPdifXlo = updateDto.EmPdifXlo;
        existingParameterSet.EmPdifVlo = updateDto.EmPdifVlo;
        existingParameterSet.EmPdifLo = updateDto.EmPdifLo;
        existingParameterSet.EmPdifNom = updateDto.EmPdifNom;
        existingParameterSet.EmPdifHi = updateDto.EmPdifHi;
        existingParameterSet.EmPdifVhi = updateDto.EmPdifVhi;
        existingParameterSet.EmPdifXhi = updateDto.EmPdifXhi;

        existingParameterSet.EmPrexXlo = updateDto.EmPrexXlo;
        existingParameterSet.EmPrexVlo = updateDto.EmPrexVlo;
        existingParameterSet.EmPrexLo = updateDto.EmPrexLo;
        existingParameterSet.EmPrexNom = updateDto.EmPrexNom;
        existingParameterSet.EmPrexHi = updateDto.EmPrexHi;
        existingParameterSet.EmPrexVhi = updateDto.EmPrexVhi;
        existingParameterSet.EmPrexXhi = updateDto.EmPrexXhi;

        existingParameterSet.EmRuseXlo = updateDto.EmRuseXlo;
        existingParameterSet.EmRuseVlo = updateDto.EmRuseVlo;
        existingParameterSet.EmRuseLo = updateDto.EmRuseLo;
        existingParameterSet.EmRuseNom = updateDto.EmRuseNom;
        existingParameterSet.EmRuseHi = updateDto.EmRuseHi;
        existingParameterSet.EmRuseVhi = updateDto.EmRuseVhi;
        existingParameterSet.EmRuseXhi = updateDto.EmRuseXhi;

        existingParameterSet.EmFcilXlo = updateDto.EmFcilXlo;
        existingParameterSet.EmFcilVlo = updateDto.EmFcilVlo;
        existingParameterSet.EmFcilLo = updateDto.EmFcilLo;
        existingParameterSet.EmFcilNom = updateDto.EmFcilNom;
        existingParameterSet.EmFcilHi = updateDto.EmFcilHi;
        existingParameterSet.EmFcilVhi = updateDto.EmFcilVhi;
        existingParameterSet.EmFcilXhi = updateDto.EmFcilXhi;

        existingParameterSet.EmScedXlo = updateDto.EmScedXlo;
        existingParameterSet.EmScedVlo = updateDto.EmScedVlo;
        existingParameterSet.EmScedLo = updateDto.EmScedLo;
        existingParameterSet.EmScedNom = updateDto.EmScedNom;
        existingParameterSet.EmScedHi = updateDto.EmScedHi;
        existingParameterSet.EmScedVhi = updateDto.EmScedVhi;
        existingParameterSet.EmScedXhi = updateDto.EmScedXhi;

        var updatedParameterSet = await _parameterSetRepository.UpdateAsync(existingParameterSet);
        _logger.LogInformation("Parameter set updated: {SetName} for user {UserId}", updatedParameterSet.SetName, userId);

        return MapToDto(updatedParameterSet);
    }

    public async Task<bool> DeleteParameterSetAsync(int paramSetId, int userId)
    {
        var parameterSet = await _parameterSetRepository.GetByIdAsync(paramSetId);

        if (parameterSet == null)
            return false;

        // Users can only delete their own parameter sets, not default ones
        if (parameterSet.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own parameter sets");

        // Check if the parameter set is being used by any estimations
        var estimationsUsingThisSet = await _estimationRepository.GetByParameterSetIdAsync(paramSetId);
        var estimationCount = estimationsUsingThisSet.Count();
        _logger.LogInformation("Parameter set {ParamSetId} has {EstimationCount} estimations", paramSetId, estimationCount);
        if (estimationCount > 0)
        {
            _logger.LogWarning("Attempting to delete parameter set {ParamSetId} with {EstimationCount} estimations", paramSetId, estimationCount);
            throw new InvalidOperationException($"Cannot delete parameter set '{parameterSet.SetName}' because it is being used by {estimationCount} estimation(s). Please reassign or delete these estimations first.");
        }

        var result = await _parameterSetRepository.DeleteAsync(paramSetId);
        if (result)
            _logger.LogInformation("Parameter set deleted: {ParamSetId} for user {UserId}", paramSetId, userId);

        return result;
    }

    public async Task<bool> ParameterSetExistsAndBelongsToUserAsync(int paramSetId, int userId)
    {
        return await _parameterSetRepository.ExistsAndBelongsToUserAsync(paramSetId, userId);
    }

    private static ParameterSetDto MapToDto(ParameterSet parameterSet)
    {
        return new ParameterSetDto
        {
            ParamSetId = parameterSet.ParamSetId,
            UserId = parameterSet.UserId,
            SetName = parameterSet.SetName,
            IsDefault = parameterSet.IsDefault,

            ConstA = parameterSet.ConstA,
            ConstB = parameterSet.ConstB,
            ConstC = parameterSet.ConstC,
            ConstD = parameterSet.ConstD,

            // Scale Factors
            SfPrecVlo = parameterSet.SfPrecVlo,
            SfPrecLo = parameterSet.SfPrecLo,
            SfPrecNom = parameterSet.SfPrecNom,
            SfPrecHi = parameterSet.SfPrecHi,
            SfPrecVhi = parameterSet.SfPrecVhi,
            SfPrecXhi = parameterSet.SfPrecXhi,

            SfFlexVlo = parameterSet.SfFlexVlo,
            SfFlexLo = parameterSet.SfFlexLo,
            SfFlexNom = parameterSet.SfFlexNom,
            SfFlexHi = parameterSet.SfFlexHi,
            SfFlexVhi = parameterSet.SfFlexVhi,
            SfFlexXhi = parameterSet.SfFlexXhi,

            SfReslVlo = parameterSet.SfReslVlo,
            SfReslLo = parameterSet.SfReslLo,
            SfReslNom = parameterSet.SfReslNom,
            SfReslHi = parameterSet.SfReslHi,
            SfReslVhi = parameterSet.SfReslVhi,
            SfReslXhi = parameterSet.SfReslXhi,

            SfTeamVlo = parameterSet.SfTeamVlo,
            SfTeamLo = parameterSet.SfTeamLo,
            SfTeamNom = parameterSet.SfTeamNom,
            SfTeamHi = parameterSet.SfTeamHi,
            SfTeamVhi = parameterSet.SfTeamVhi,
            SfTeamXhi = parameterSet.SfTeamXhi,

            SfPmatVlo = parameterSet.SfPmatVlo,
            SfPmatLo = parameterSet.SfPmatLo,
            SfPmatNom = parameterSet.SfPmatNom,
            SfPmatHi = parameterSet.SfPmatHi,
            SfPmatVhi = parameterSet.SfPmatVhi,
            SfPmatXhi = parameterSet.SfPmatXhi,

            // Effort Multipliers
            EmPersXlo = parameterSet.EmPersXlo,
            EmPersVlo = parameterSet.EmPersVlo,
            EmPersLo = parameterSet.EmPersLo,
            EmPersNom = parameterSet.EmPersNom,
            EmPersHi = parameterSet.EmPersHi,
            EmPersVhi = parameterSet.EmPersVhi,
            EmPersXhi = parameterSet.EmPersXhi,

            EmRcpxXlo = parameterSet.EmRcpxXlo,
            EmRcpxVlo = parameterSet.EmRcpxVlo,
            EmRcpxLo = parameterSet.EmRcpxLo,
            EmRcpxNom = parameterSet.EmRcpxNom,
            EmRcpxHi = parameterSet.EmRcpxHi,
            EmRcpxVhi = parameterSet.EmRcpxVhi,
            EmRcpxXhi = parameterSet.EmRcpxXhi,

            EmPdifXlo = parameterSet.EmPdifXlo,
            EmPdifVlo = parameterSet.EmPdifVlo,
            EmPdifLo = parameterSet.EmPdifLo,
            EmPdifNom = parameterSet.EmPdifNom,
            EmPdifHi = parameterSet.EmPdifHi,
            EmPdifVhi = parameterSet.EmPdifVhi,
            EmPdifXhi = parameterSet.EmPdifXhi,

            EmPrexXlo = parameterSet.EmPrexXlo,
            EmPrexVlo = parameterSet.EmPrexVlo,
            EmPrexLo = parameterSet.EmPrexLo,
            EmPrexNom = parameterSet.EmPrexNom,
            EmPrexHi = parameterSet.EmPrexHi,
            EmPrexVhi = parameterSet.EmPrexVhi,
            EmPrexXhi = parameterSet.EmPrexXhi,

            EmRuseXlo = parameterSet.EmRuseXlo,
            EmRuseVlo = parameterSet.EmRuseVlo,
            EmRuseLo = parameterSet.EmRuseLo,
            EmRuseNom = parameterSet.EmRuseNom,
            EmRuseHi = parameterSet.EmRuseHi,
            EmRuseVhi = parameterSet.EmRuseVhi,
            EmRuseXhi = parameterSet.EmRuseXhi,

            EmFcilXlo = parameterSet.EmFcilXlo,
            EmFcilVlo = parameterSet.EmFcilVlo,
            EmFcilLo = parameterSet.EmFcilLo,
            EmFcilNom = parameterSet.EmFcilNom,
            EmFcilHi = parameterSet.EmFcilHi,
            EmFcilVhi = parameterSet.EmFcilVhi,
            EmFcilXhi = parameterSet.EmFcilXhi,

            EmScedXlo = parameterSet.EmScedXlo,
            EmScedVlo = parameterSet.EmScedVlo,
            EmScedLo = parameterSet.EmScedLo,
            EmScedNom = parameterSet.EmScedNom,
            EmScedHi = parameterSet.EmScedHi,
            EmScedVhi = parameterSet.EmScedVhi,
            EmScedXhi = parameterSet.EmScedXhi
        };
    }

    private void ValidateSFValues(CreateParameterSetDto createDto)
    {
        var missingValues = new List<string>();

        // Check PREC values
        if (!createDto.SfPrecVlo.HasValue) missingValues.Add("PREC/VLO");
        if (!createDto.SfPrecLo.HasValue) missingValues.Add("PREC/LO");
        if (!createDto.SfPrecNom.HasValue) missingValues.Add("PREC/NOM");
        if (!createDto.SfPrecHi.HasValue) missingValues.Add("PREC/HI");
        if (!createDto.SfPrecVhi.HasValue) missingValues.Add("PREC/VHI");
        if (!createDto.SfPrecXhi.HasValue) missingValues.Add("PREC/XHI");

        // Check FLEX values
        if (!createDto.SfFlexVlo.HasValue) missingValues.Add("FLEX/VLO");
        if (!createDto.SfFlexLo.HasValue) missingValues.Add("FLEX/LO");
        if (!createDto.SfFlexNom.HasValue) missingValues.Add("FLEX/NOM");
        if (!createDto.SfFlexHi.HasValue) missingValues.Add("FLEX/HI");
        if (!createDto.SfFlexVhi.HasValue) missingValues.Add("FLEX/VHI");
        if (!createDto.SfFlexXhi.HasValue) missingValues.Add("FLEX/XHI");

        // Check RESL values
        if (!createDto.SfReslVlo.HasValue) missingValues.Add("RESL/VLO");
        if (!createDto.SfReslLo.HasValue) missingValues.Add("RESL/LO");
        if (!createDto.SfReslNom.HasValue) missingValues.Add("RESL/NOM");
        if (!createDto.SfReslHi.HasValue) missingValues.Add("RESL/HI");
        if (!createDto.SfReslVhi.HasValue) missingValues.Add("RESL/VHI");
        if (!createDto.SfReslXhi.HasValue) missingValues.Add("RESL/XHI");

        // Check TEAM values
        if (!createDto.SfTeamVlo.HasValue) missingValues.Add("TEAM/VLO");
        if (!createDto.SfTeamLo.HasValue) missingValues.Add("TEAM/LO");
        if (!createDto.SfTeamNom.HasValue) missingValues.Add("TEAM/NOM");
        if (!createDto.SfTeamHi.HasValue) missingValues.Add("TEAM/HI");
        if (!createDto.SfTeamVhi.HasValue) missingValues.Add("TEAM/VHI");
        if (!createDto.SfTeamXhi.HasValue) missingValues.Add("TEAM/XHI");

        // Check PMAT values
        if (!createDto.SfPmatVlo.HasValue) missingValues.Add("PMAT/VLO");
        if (!createDto.SfPmatLo.HasValue) missingValues.Add("PMAT/LO");
        if (!createDto.SfPmatNom.HasValue) missingValues.Add("PMAT/NOM");
        if (!createDto.SfPmatHi.HasValue) missingValues.Add("PMAT/HI");
        if (!createDto.SfPmatVhi.HasValue) missingValues.Add("PMAT/VHI");
        if (!createDto.SfPmatXhi.HasValue) missingValues.Add("PMAT/XHI");

        if (missingValues.Any())
        {
            throw new ArgumentException($"All Scale Factor values must be provided. Missing values: {string.Join(", ", missingValues)}");
        }
    }

    private void ValidateSFValuesForUpdate(UpdateParameterSetDto updateDto)
    {
        var missingValues = new List<string>();

        // Check PREC values
        if (!updateDto.SfPrecVlo.HasValue) missingValues.Add("PREC/VLO");
        if (!updateDto.SfPrecLo.HasValue) missingValues.Add("PREC/LO");
        if (!updateDto.SfPrecNom.HasValue) missingValues.Add("PREC/NOM");
        if (!updateDto.SfPrecHi.HasValue) missingValues.Add("PREC/HI");
        if (!updateDto.SfPrecVhi.HasValue) missingValues.Add("PREC/VHI");
        if (!updateDto.SfPrecXhi.HasValue) missingValues.Add("PREC/XHI");

        // Check FLEX values
        if (!updateDto.SfFlexVlo.HasValue) missingValues.Add("FLEX/VLO");
        if (!updateDto.SfFlexLo.HasValue) missingValues.Add("FLEX/LO");
        if (!updateDto.SfFlexNom.HasValue) missingValues.Add("FLEX/NOM");
        if (!updateDto.SfFlexHi.HasValue) missingValues.Add("FLEX/HI");
        if (!updateDto.SfFlexVhi.HasValue) missingValues.Add("FLEX/VHI");
        if (!updateDto.SfFlexXhi.HasValue) missingValues.Add("FLEX/XHI");

        // Check RESL values
        if (!updateDto.SfReslVlo.HasValue) missingValues.Add("RESL/VLO");
        if (!updateDto.SfReslLo.HasValue) missingValues.Add("RESL/LO");
        if (!updateDto.SfReslNom.HasValue) missingValues.Add("RESL/NOM");
        if (!updateDto.SfReslHi.HasValue) missingValues.Add("RESL/HI");
        if (!updateDto.SfReslVhi.HasValue) missingValues.Add("RESL/VHI");
        if (!updateDto.SfReslXhi.HasValue) missingValues.Add("RESL/XHI");

        // Check TEAM values
        if (!updateDto.SfTeamVlo.HasValue) missingValues.Add("TEAM/VLO");
        if (!updateDto.SfTeamLo.HasValue) missingValues.Add("TEAM/LO");
        if (!updateDto.SfTeamNom.HasValue) missingValues.Add("TEAM/NOM");
        if (!updateDto.SfTeamHi.HasValue) missingValues.Add("TEAM/HI");
        if (!updateDto.SfTeamVhi.HasValue) missingValues.Add("TEAM/VHI");
        if (!updateDto.SfTeamXhi.HasValue) missingValues.Add("TEAM/XHI");

        // Check PMAT values
        if (!updateDto.SfPmatVlo.HasValue) missingValues.Add("PMAT/VLO");
        if (!updateDto.SfPmatLo.HasValue) missingValues.Add("PMAT/LO");
        if (!updateDto.SfPmatNom.HasValue) missingValues.Add("PMAT/NOM");
        if (!updateDto.SfPmatHi.HasValue) missingValues.Add("PMAT/HI");
        if (!updateDto.SfPmatVhi.HasValue) missingValues.Add("PMAT/VHI");
        if (!updateDto.SfPmatXhi.HasValue) missingValues.Add("PMAT/XHI");

        if (missingValues.Any())
        {
            throw new ArgumentException($"All Scale Factor values must be provided. Missing values: {string.Join(", ", missingValues)}");
        }
    }
}