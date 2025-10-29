using Backend.Models.DTOs;
using Backend.Models.Entities;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

/// <summary>
/// Implementation of ParameterSet service
/// </summary>
public class ParameterSetService : IParameterSetService
{
    private readonly IParameterSetRepository _parameterSetRepository;
    private readonly ILogger<ParameterSetService> _logger;

    public ParameterSetService(
        IParameterSetRepository parameterSetRepository,
        ILogger<ParameterSetService> logger)
    {
        _parameterSetRepository = parameterSetRepository;
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
}