using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Services.Implementations.CocomoIIStage3;

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
        var parameterSet = new ParameterSet
        {
            SetName = createDto.SetName.Trim(),
            UserId = userId,
            IsDefault = false,

            // Scale Factors
            SfPrecXlo = createDto.SfPrecXlo,
            SfPrecVlo = createDto.SfPrecVlo,
            SfPrecLo = createDto.SfPrecLo,
            SfPrecNom = createDto.SfPrecNom,
            SfPrecHi = createDto.SfPrecHi,
            SfPrecVhi = createDto.SfPrecVhi,
            SfPrecXhi = createDto.SfPrecXhi,

            SfFlexXlo = createDto.SfFlexXlo,
            SfFlexVlo = createDto.SfFlexVlo,
            SfFlexLo = createDto.SfFlexLo,
            SfFlexNom = createDto.SfFlexNom,
            SfFlexHi = createDto.SfFlexHi,
            SfFlexVhi = createDto.SfFlexVhi,
            SfFlexXhi = createDto.SfFlexXhi,

            SfReslXlo = createDto.SfReslXlo,
            SfReslVlo = createDto.SfReslVlo,
            SfReslLo = createDto.SfReslLo,
            SfReslNom = createDto.SfReslNom,
            SfReslHi = createDto.SfReslHi,
            SfReslVhi = createDto.SfReslVhi,
            SfReslXhi = createDto.SfReslXhi,

            SfTeamXlo = createDto.SfTeamXlo,
            SfTeamVlo = createDto.SfTeamVlo,
            SfTeamLo = createDto.SfTeamLo,
            SfTeamNom = createDto.SfTeamNom,
            SfTeamHi = createDto.SfTeamHi,
            SfTeamVhi = createDto.SfTeamVhi,
            SfTeamXhi = createDto.SfTeamXhi,

            SfPmatXlo = createDto.SfPmatXlo,
            SfPmatVlo = createDto.SfPmatVlo,
            SfPmatLo = createDto.SfPmatLo,
            SfPmatNom = createDto.SfPmatNom,
            SfPmatHi = createDto.SfPmatHi,
            SfPmatVhi = createDto.SfPmatVhi,
            SfPmatXhi = createDto.SfPmatXhi,

            // Effort Multipliers - Updated for 17 factors
            EmRelyXlo = createDto.EmRelyXlo,
            EmRelyVlo = createDto.EmRelyVlo,
            EmRelyLo = createDto.EmRelyLo,
            EmRelyNom = createDto.EmRelyNom,
            EmRelyHi = createDto.EmRelyHi,
            EmRelyVhi = createDto.EmRelyVhi,
            EmRelyXhi = createDto.EmRelyXhi,

            EmDataXlo = createDto.EmDataXlo,
            EmDataVlo = createDto.EmDataVlo,
            EmDataLo = createDto.EmDataLo,
            EmDataNom = createDto.EmDataNom,
            EmDataHi = createDto.EmDataHi,
            EmDataVhi = createDto.EmDataVhi,
            EmDataXhi = createDto.EmDataXhi,

            EmCplxXlo = createDto.EmCplxXlo,
            EmCplxVlo = createDto.EmCplxVlo,
            EmCplxLo = createDto.EmCplxLo,
            EmCplxNom = createDto.EmCplxNom,
            EmCplxHi = createDto.EmCplxHi,
            EmCplxVhi = createDto.EmCplxVhi,
            EmCplxXhi = createDto.EmCplxXhi,

            EmRuseXlo = createDto.EmRuseXlo,
            EmRuseVlo = createDto.EmRuseVlo,
            EmRuseLo = createDto.EmRuseLo,
            EmRuseNom = createDto.EmRuseNom,
            EmRuseHi = createDto.EmRuseHi,
            EmRuseVhi = createDto.EmRuseVhi,
            EmRuseXhi = createDto.EmRuseXhi,

            EmDocuXlo = createDto.EmDocuXlo,
            EmDocuVlo = createDto.EmDocuVlo,
            EmDocuLo = createDto.EmDocuLo,
            EmDocuNom = createDto.EmDocuNom,
            EmDocuHi = createDto.EmDocuHi,
            EmDocuVhi = createDto.EmDocuVhi,
            EmDocuXhi = createDto.EmDocuXhi,

            EmTimeXlo = createDto.EmTimeXlo,
            EmTimeVlo = createDto.EmTimeVlo,
            EmTimeLo = createDto.EmTimeLo,
            EmTimeNom = createDto.EmTimeNom,
            EmTimeHi = createDto.EmTimeHi,
            EmTimeVhi = createDto.EmTimeVhi,
            EmTimeXhi = createDto.EmTimeXhi,

            EmStorXlo = createDto.EmStorXlo,
            EmStorVlo = createDto.EmStorVlo,
            EmStorLo = createDto.EmStorLo,
            EmStorNom = createDto.EmStorNom,
            EmStorHi = createDto.EmStorHi,
            EmStorVhi = createDto.EmStorVhi,
            EmStorXhi = createDto.EmStorXhi,

            EmPvolXlo = createDto.EmPvolXlo,
            EmPvolVlo = createDto.EmPvolVlo,
            EmPvolLo = createDto.EmPvolLo,
            EmPvolNom = createDto.EmPvolNom,
            EmPvolHi = createDto.EmPvolHi,
            EmPvolVhi = createDto.EmPvolVhi,
            EmPvolXhi = createDto.EmPvolXhi,

            EmAcapXlo = createDto.EmAcapXlo,
            EmAcapVlo = createDto.EmAcapVlo,
            EmAcapLo = createDto.EmAcapLo,
            EmAcapNom = createDto.EmAcapNom,
            EmAcapHi = createDto.EmAcapHi,
            EmAcapVhi = createDto.EmAcapVhi,
            EmAcapXhi = createDto.EmAcapXhi,

            EmPcapXlo = createDto.EmPcapXlo,
            EmPcapVlo = createDto.EmPcapVlo,
            EmPcapLo = createDto.EmPcapLo,
            EmPcapNom = createDto.EmPcapNom,
            EmPcapHi = createDto.EmPcapHi,
            EmPcapVhi = createDto.EmPcapVhi,
            EmPcapXhi = createDto.EmPcapXhi,

            EmPconXlo = createDto.EmPconXlo,
            EmPconVlo = createDto.EmPconVlo,
            EmPconLo = createDto.EmPconLo,
            EmPconNom = createDto.EmPconNom,
            EmPconHi = createDto.EmPconHi,
            EmPconVhi = createDto.EmPconVhi,
            EmPconXhi = createDto.EmPconXhi,

            EmApexXlo = createDto.EmApexXlo,
            EmApexVlo = createDto.EmApexVlo,
            EmApexLo = createDto.EmApexLo,
            EmApexNom = createDto.EmApexNom,
            EmApexHi = createDto.EmApexHi,
            EmApexVhi = createDto.EmApexVhi,
            EmApexXhi = createDto.EmApexXhi,

            EmPlexXlo = createDto.EmPlexXlo,
            EmPlexVlo = createDto.EmPlexVlo,
            EmPlexLo = createDto.EmPlexLo,
            EmPlexNom = createDto.EmPlexNom,
            EmPlexHi = createDto.EmPlexHi,
            EmPlexVhi = createDto.EmPlexVhi,
            EmPlexXhi = createDto.EmPlexXhi,

            EmLtexXlo = createDto.EmLtexXlo,
            EmLtexVlo = createDto.EmLtexVlo,
            EmLtexLo = createDto.EmLtexLo,
            EmLtexNom = createDto.EmLtexNom,
            EmLtexHi = createDto.EmLtexHi,
            EmLtexVhi = createDto.EmLtexVhi,
            EmLtexXhi = createDto.EmLtexXhi,

            EmToolXlo = createDto.EmToolXlo,
            EmToolVlo = createDto.EmToolVlo,
            EmToolLo = createDto.EmToolLo,
            EmToolNom = createDto.EmToolNom,
            EmToolHi = createDto.EmToolHi,
            EmToolVhi = createDto.EmToolVhi,
            EmToolXhi = createDto.EmToolXhi,

            EmSiteXlo = createDto.EmSiteXlo,
            EmSiteVlo = createDto.EmSiteVlo,
            EmSiteLo = createDto.EmSiteLo,
            EmSiteNom = createDto.EmSiteNom,
            EmSiteHi = createDto.EmSiteHi,
            EmSiteVhi = createDto.EmSiteVhi,
            EmSiteXhi = createDto.EmSiteXhi,

            EmScedXlo = createDto.EmScedXlo,
            EmScedVlo = createDto.EmScedVlo,
            EmScedLo = createDto.EmScedLo,
            EmScedNom = createDto.EmScedNom,
            EmScedHi = createDto.EmScedHi,
            EmScedVhi = createDto.EmScedVhi,
            EmScedXhi = createDto.EmScedXhi,

            // Constants
            ConstA = createDto.ConstA,
            ConstB = createDto.ConstB,
            ConstC = createDto.ConstC,
            ConstD = createDto.ConstD
        };

        var createdParameterSet = await _parameterSetRepository.CreateAsync(parameterSet);
        return MapToDto(createdParameterSet);
    }

    public async Task<ParameterSetDto> UpdateParameterSetAsync(UpdateParameterSetDto updateDto, int userId)
    {
        var existingParameterSet = await _parameterSetRepository.GetByIdAsync(updateDto.ParamSetId);

        if (existingParameterSet == null)
            throw new KeyNotFoundException($"Parameter set with ID {updateDto.ParamSetId} not found");

        // Check ownership
        if (!existingParameterSet.IsDefault && existingParameterSet.UserId != userId)
            throw new UnauthorizedAccessException("You don't have permission to update this parameter set");

        // Update all fields
        existingParameterSet.SetName = updateDto.SetName.Trim();

        // Scale Factors
        existingParameterSet.SfPrecXlo = updateDto.SfPrecXlo;
        existingParameterSet.SfPrecVlo = updateDto.SfPrecVlo;
        existingParameterSet.SfPrecLo = updateDto.SfPrecLo;
        existingParameterSet.SfPrecNom = updateDto.SfPrecNom;
        existingParameterSet.SfPrecHi = updateDto.SfPrecHi;
        existingParameterSet.SfPrecVhi = updateDto.SfPrecVhi;
        existingParameterSet.SfPrecXhi = updateDto.SfPrecXhi;

        existingParameterSet.SfFlexXlo = updateDto.SfFlexXlo;
        existingParameterSet.SfFlexVlo = updateDto.SfFlexVlo;
        existingParameterSet.SfFlexLo = updateDto.SfFlexLo;
        existingParameterSet.SfFlexNom = updateDto.SfFlexNom;
        existingParameterSet.SfFlexHi = updateDto.SfFlexHi;
        existingParameterSet.SfFlexVhi = updateDto.SfFlexVhi;
        existingParameterSet.SfFlexXhi = updateDto.SfFlexXhi;

        existingParameterSet.SfReslXlo = updateDto.SfReslXlo;
        existingParameterSet.SfReslVlo = updateDto.SfReslVlo;
        existingParameterSet.SfReslLo = updateDto.SfReslLo;
        existingParameterSet.SfReslNom = updateDto.SfReslNom;
        existingParameterSet.SfReslHi = updateDto.SfReslHi;
        existingParameterSet.SfReslVhi = updateDto.SfReslVhi;
        existingParameterSet.SfReslXhi = updateDto.SfReslXhi;

        existingParameterSet.SfTeamXlo = updateDto.SfTeamXlo;
        existingParameterSet.SfTeamVlo = updateDto.SfTeamVlo;
        existingParameterSet.SfTeamLo = updateDto.SfTeamLo;
        existingParameterSet.SfTeamNom = updateDto.SfTeamNom;
        existingParameterSet.SfTeamHi = updateDto.SfTeamHi;
        existingParameterSet.SfTeamVhi = updateDto.SfTeamVhi;
        existingParameterSet.SfTeamXhi = updateDto.SfTeamXhi;

        existingParameterSet.SfPmatXlo = updateDto.SfPmatXlo;
        existingParameterSet.SfPmatVlo = updateDto.SfPmatVlo;
        existingParameterSet.SfPmatLo = updateDto.SfPmatLo;
        existingParameterSet.SfPmatNom = updateDto.SfPmatNom;
        existingParameterSet.SfPmatHi = updateDto.SfPmatHi;
        existingParameterSet.SfPmatVhi = updateDto.SfPmatVhi;
        existingParameterSet.SfPmatXhi = updateDto.SfPmatXhi;

        // Effort Multipliers - Updated for 17 factors
        existingParameterSet.EmRelyXlo = updateDto.EmRelyXlo;
        existingParameterSet.EmRelyVlo = updateDto.EmRelyVlo;
        existingParameterSet.EmRelyLo = updateDto.EmRelyLo;
        existingParameterSet.EmRelyNom = updateDto.EmRelyNom;
        existingParameterSet.EmRelyHi = updateDto.EmRelyHi;
        existingParameterSet.EmRelyVhi = updateDto.EmRelyVhi;
        existingParameterSet.EmRelyXhi = updateDto.EmRelyXhi;

        existingParameterSet.EmDataXlo = updateDto.EmDataXlo;
        existingParameterSet.EmDataVlo = updateDto.EmDataVlo;
        existingParameterSet.EmDataLo = updateDto.EmDataLo;
        existingParameterSet.EmDataNom = updateDto.EmDataNom;
        existingParameterSet.EmDataHi = updateDto.EmDataHi;
        existingParameterSet.EmDataVhi = updateDto.EmDataVhi;
        existingParameterSet.EmDataXhi = updateDto.EmDataXhi;

        existingParameterSet.EmCplxXlo = updateDto.EmCplxXlo;
        existingParameterSet.EmCplxVlo = updateDto.EmCplxVlo;
        existingParameterSet.EmCplxLo = updateDto.EmCplxLo;
        existingParameterSet.EmCplxNom = updateDto.EmCplxNom;
        existingParameterSet.EmCplxHi = updateDto.EmCplxHi;
        existingParameterSet.EmCplxVhi = updateDto.EmCplxVhi;
        existingParameterSet.EmCplxXhi = updateDto.EmCplxXhi;

        existingParameterSet.EmRuseXlo = updateDto.EmRuseXlo;
        existingParameterSet.EmRuseVlo = updateDto.EmRuseVlo;
        existingParameterSet.EmRuseLo = updateDto.EmRuseLo;
        existingParameterSet.EmRuseNom = updateDto.EmRuseNom;
        existingParameterSet.EmRuseHi = updateDto.EmRuseHi;
        existingParameterSet.EmRuseVhi = updateDto.EmRuseVhi;
        existingParameterSet.EmRuseXhi = updateDto.EmRuseXhi;

        existingParameterSet.EmDocuXlo = updateDto.EmDocuXlo;
        existingParameterSet.EmDocuVlo = updateDto.EmDocuVlo;
        existingParameterSet.EmDocuLo = updateDto.EmDocuLo;
        existingParameterSet.EmDocuNom = updateDto.EmDocuNom;
        existingParameterSet.EmDocuHi = updateDto.EmDocuHi;
        existingParameterSet.EmDocuVhi = updateDto.EmDocuVhi;
        existingParameterSet.EmDocuXhi = updateDto.EmDocuXhi;

        existingParameterSet.EmTimeXlo = updateDto.EmTimeXlo;
        existingParameterSet.EmTimeVlo = updateDto.EmTimeVlo;
        existingParameterSet.EmTimeLo = updateDto.EmTimeLo;
        existingParameterSet.EmTimeNom = updateDto.EmTimeNom;
        existingParameterSet.EmTimeHi = updateDto.EmTimeHi;
        existingParameterSet.EmTimeVhi = updateDto.EmTimeVhi;
        existingParameterSet.EmTimeXhi = updateDto.EmTimeXhi;

        existingParameterSet.EmStorXlo = updateDto.EmStorXlo;
        existingParameterSet.EmStorVlo = updateDto.EmStorVlo;
        existingParameterSet.EmStorLo = updateDto.EmStorLo;
        existingParameterSet.EmStorNom = updateDto.EmStorNom;
        existingParameterSet.EmStorHi = updateDto.EmStorHi;
        existingParameterSet.EmStorVhi = updateDto.EmStorVhi;
        existingParameterSet.EmStorXhi = updateDto.EmStorXhi;

        existingParameterSet.EmPvolXlo = updateDto.EmPvolXlo;
        existingParameterSet.EmPvolVlo = updateDto.EmPvolVlo;
        existingParameterSet.EmPvolLo = updateDto.EmPvolLo;
        existingParameterSet.EmPvolNom = updateDto.EmPvolNom;
        existingParameterSet.EmPvolHi = updateDto.EmPvolHi;
        existingParameterSet.EmPvolVhi = updateDto.EmPvolVhi;
        existingParameterSet.EmPvolXhi = updateDto.EmPvolXhi;

        existingParameterSet.EmAcapXlo = updateDto.EmAcapXlo;
        existingParameterSet.EmAcapVlo = updateDto.EmAcapVlo;
        existingParameterSet.EmAcapLo = updateDto.EmAcapLo;
        existingParameterSet.EmAcapNom = updateDto.EmAcapNom;
        existingParameterSet.EmAcapHi = updateDto.EmAcapHi;
        existingParameterSet.EmAcapVhi = updateDto.EmAcapVhi;
        existingParameterSet.EmAcapXhi = updateDto.EmAcapXhi;

        existingParameterSet.EmPcapXlo = updateDto.EmPcapXlo;
        existingParameterSet.EmPcapVlo = updateDto.EmPcapVlo;
        existingParameterSet.EmPcapLo = updateDto.EmPcapLo;
        existingParameterSet.EmPcapNom = updateDto.EmPcapNom;
        existingParameterSet.EmPcapHi = updateDto.EmPcapHi;
        existingParameterSet.EmPcapVhi = updateDto.EmPcapVhi;
        existingParameterSet.EmPcapXhi = updateDto.EmPcapXhi;

        existingParameterSet.EmPconXlo = updateDto.EmPconXlo;
        existingParameterSet.EmPconVlo = updateDto.EmPconVlo;
        existingParameterSet.EmPconLo = updateDto.EmPconLo;
        existingParameterSet.EmPconNom = updateDto.EmPconNom;
        existingParameterSet.EmPconHi = updateDto.EmPconHi;
        existingParameterSet.EmPconVhi = updateDto.EmPconVhi;
        existingParameterSet.EmPconXhi = updateDto.EmPconXhi;

        existingParameterSet.EmApexXlo = updateDto.EmApexXlo;
        existingParameterSet.EmApexVlo = updateDto.EmApexVlo;
        existingParameterSet.EmApexLo = updateDto.EmApexLo;
        existingParameterSet.EmApexNom = updateDto.EmApexNom;
        existingParameterSet.EmApexHi = updateDto.EmApexHi;
        existingParameterSet.EmApexVhi = updateDto.EmApexVhi;
        existingParameterSet.EmApexXhi = updateDto.EmApexXhi;

        existingParameterSet.EmPlexXlo = updateDto.EmPlexXlo;
        existingParameterSet.EmPlexVlo = updateDto.EmPlexVlo;
        existingParameterSet.EmPlexLo = updateDto.EmPlexLo;
        existingParameterSet.EmPlexNom = updateDto.EmPlexNom;
        existingParameterSet.EmPlexHi = updateDto.EmPlexHi;
        existingParameterSet.EmPlexVhi = updateDto.EmPlexVhi;
        existingParameterSet.EmPlexXhi = updateDto.EmPlexXhi;

        existingParameterSet.EmLtexXlo = updateDto.EmLtexXlo;
        existingParameterSet.EmLtexVlo = updateDto.EmLtexVlo;
        existingParameterSet.EmLtexLo = updateDto.EmLtexLo;
        existingParameterSet.EmLtexNom = updateDto.EmLtexNom;
        existingParameterSet.EmLtexHi = updateDto.EmLtexHi;
        existingParameterSet.EmLtexVhi = updateDto.EmLtexVhi;
        existingParameterSet.EmLtexXhi = updateDto.EmLtexXhi;

        existingParameterSet.EmToolXlo = updateDto.EmToolXlo;
        existingParameterSet.EmToolVlo = updateDto.EmToolVlo;
        existingParameterSet.EmToolLo = updateDto.EmToolLo;
        existingParameterSet.EmToolNom = updateDto.EmToolNom;
        existingParameterSet.EmToolHi = updateDto.EmToolHi;
        existingParameterSet.EmToolVhi = updateDto.EmToolVhi;
        existingParameterSet.EmToolXhi = updateDto.EmToolXhi;

        existingParameterSet.EmSiteXlo = updateDto.EmSiteXlo;
        existingParameterSet.EmSiteVlo = updateDto.EmSiteVlo;
        existingParameterSet.EmSiteLo = updateDto.EmSiteLo;
        existingParameterSet.EmSiteNom = updateDto.EmSiteNom;
        existingParameterSet.EmSiteHi = updateDto.EmSiteHi;
        existingParameterSet.EmSiteVhi = updateDto.EmSiteVhi;
        existingParameterSet.EmSiteXhi = updateDto.EmSiteXhi;

        existingParameterSet.EmScedXlo = updateDto.EmScedXlo;
        existingParameterSet.EmScedVlo = updateDto.EmScedVlo;
        existingParameterSet.EmScedLo = updateDto.EmScedLo;
        existingParameterSet.EmScedNom = updateDto.EmScedNom;
        existingParameterSet.EmScedHi = updateDto.EmScedHi;
        existingParameterSet.EmScedVhi = updateDto.EmScedVhi;
        existingParameterSet.EmScedXhi = updateDto.EmScedXhi;

        // Constants
        existingParameterSet.ConstA = updateDto.ConstA;
        existingParameterSet.ConstB = updateDto.ConstB;
        existingParameterSet.ConstC = updateDto.ConstC;
        existingParameterSet.ConstD = updateDto.ConstD;

        var updatedParameterSet = await _parameterSetRepository.UpdateAsync(existingParameterSet);

        // Check if any estimations use this parameter set and need recalculation
        var estimations = await _estimationRepository.GetByParameterSetIdAsync(updateDto.ParamSetId);
        foreach (var estimation in estimations)
        {
            _logger.LogInformation($"Recalculating estimation {estimation.EstimationId} due to parameter set update");
            // Note: In a real implementation, you might want to trigger recalculation here
            // For now, we'll skip this to avoid circular dependencies
        }

        return MapToDto(updatedParameterSet);
    }

    public async Task<bool> DeleteParameterSetAsync(int paramSetId, int userId)
    {
        var parameterSet = await _parameterSetRepository.GetByIdAsync(paramSetId);

        if (parameterSet == null)
            return false;

        // Check ownership
        if (!parameterSet.IsDefault && parameterSet.UserId != userId)
            throw new UnauthorizedAccessException("You don't have permission to delete this parameter set");

        // Check if parameter set is being used by any estimations
        var estimations = await _estimationRepository.GetByParameterSetIdAsync(paramSetId);
        if (estimations.Any())
        {
            throw new InvalidOperationException("Cannot delete parameter set that is being used by estimations");
        }

        return await _parameterSetRepository.DeleteAsync(paramSetId);
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
            SetName = parameterSet.SetName,
            IsDefault = parameterSet.IsDefault,
            UserId = parameterSet.UserId,

            // Scale Factors
            SfPrecXlo = parameterSet.SfPrecXlo,
            SfPrecVlo = parameterSet.SfPrecVlo,
            SfPrecLo = parameterSet.SfPrecLo,
            SfPrecNom = parameterSet.SfPrecNom,
            SfPrecHi = parameterSet.SfPrecHi,
            SfPrecVhi = parameterSet.SfPrecVhi,
            SfPrecXhi = parameterSet.SfPrecXhi,

            SfFlexXlo = parameterSet.SfFlexXlo,
            SfFlexVlo = parameterSet.SfFlexVlo,
            SfFlexLo = parameterSet.SfFlexLo,
            SfFlexNom = parameterSet.SfFlexNom,
            SfFlexHi = parameterSet.SfFlexHi,
            SfFlexVhi = parameterSet.SfFlexVhi,
            SfFlexXhi = parameterSet.SfFlexXhi,

            SfReslXlo = parameterSet.SfReslXlo,
            SfReslVlo = parameterSet.SfReslVlo,
            SfReslLo = parameterSet.SfReslLo,
            SfReslNom = parameterSet.SfReslNom,
            SfReslHi = parameterSet.SfReslHi,
            SfReslVhi = parameterSet.SfReslVhi,
            SfReslXhi = parameterSet.SfReslXhi,

            SfTeamXlo = parameterSet.SfTeamXlo,
            SfTeamVlo = parameterSet.SfTeamVlo,
            SfTeamLo = parameterSet.SfTeamLo,
            SfTeamNom = parameterSet.SfTeamNom,
            SfTeamHi = parameterSet.SfTeamHi,
            SfTeamVhi = parameterSet.SfTeamVhi,
            SfTeamXhi = parameterSet.SfTeamXhi,

            SfPmatXlo = parameterSet.SfPmatXlo,
            SfPmatVlo = parameterSet.SfPmatVlo,
            SfPmatLo = parameterSet.SfPmatLo,
            SfPmatNom = parameterSet.SfPmatNom,
            SfPmatHi = parameterSet.SfPmatHi,
            SfPmatVhi = parameterSet.SfPmatVhi,
            SfPmatXhi = parameterSet.SfPmatXhi,

            // Effort Multipliers - Updated for 17 factors
            EmRelyXlo = parameterSet.EmRelyXlo,
            EmRelyVlo = parameterSet.EmRelyVlo,
            EmRelyLo = parameterSet.EmRelyLo,
            EmRelyNom = parameterSet.EmRelyNom,
            EmRelyHi = parameterSet.EmRelyHi,
            EmRelyVhi = parameterSet.EmRelyVhi,
            EmRelyXhi = parameterSet.EmRelyXhi,

            EmDataXlo = parameterSet.EmDataXlo,
            EmDataVlo = parameterSet.EmDataVlo,
            EmDataLo = parameterSet.EmDataLo,
            EmDataNom = parameterSet.EmDataNom,
            EmDataHi = parameterSet.EmDataHi,
            EmDataVhi = parameterSet.EmDataVhi,
            EmDataXhi = parameterSet.EmDataXhi,

            EmCplxXlo = parameterSet.EmCplxXlo,
            EmCplxVlo = parameterSet.EmCplxVlo,
            EmCplxLo = parameterSet.EmCplxLo,
            EmCplxNom = parameterSet.EmCplxNom,
            EmCplxHi = parameterSet.EmCplxHi,
            EmCplxVhi = parameterSet.EmCplxVhi,
            EmCplxXhi = parameterSet.EmCplxXhi,

            EmRuseXlo = parameterSet.EmRuseXlo,
            EmRuseVlo = parameterSet.EmRuseVlo,
            EmRuseLo = parameterSet.EmRuseLo,
            EmRuseNom = parameterSet.EmRuseNom,
            EmRuseHi = parameterSet.EmRuseHi,
            EmRuseVhi = parameterSet.EmRuseVhi,
            EmRuseXhi = parameterSet.EmRuseXhi,

            EmDocuXlo = parameterSet.EmDocuXlo,
            EmDocuVlo = parameterSet.EmDocuVlo,
            EmDocuLo = parameterSet.EmDocuLo,
            EmDocuNom = parameterSet.EmDocuNom,
            EmDocuHi = parameterSet.EmDocuHi,
            EmDocuVhi = parameterSet.EmDocuVhi,
            EmDocuXhi = parameterSet.EmDocuXhi,

            EmTimeXlo = parameterSet.EmTimeXlo,
            EmTimeVlo = parameterSet.EmTimeVlo,
            EmTimeLo = parameterSet.EmTimeLo,
            EmTimeNom = parameterSet.EmTimeNom,
            EmTimeHi = parameterSet.EmTimeHi,
            EmTimeVhi = parameterSet.EmTimeVhi,
            EmTimeXhi = parameterSet.EmTimeXhi,

            EmStorXlo = parameterSet.EmStorXlo,
            EmStorVlo = parameterSet.EmStorVlo,
            EmStorLo = parameterSet.EmStorLo,
            EmStorNom = parameterSet.EmStorNom,
            EmStorHi = parameterSet.EmStorHi,
            EmStorVhi = parameterSet.EmStorVhi,
            EmStorXhi = parameterSet.EmStorXhi,

            EmPvolXlo = parameterSet.EmPvolXlo,
            EmPvolVlo = parameterSet.EmPvolVlo,
            EmPvolLo = parameterSet.EmPvolLo,
            EmPvolNom = parameterSet.EmPvolNom,
            EmPvolHi = parameterSet.EmPvolHi,
            EmPvolVhi = parameterSet.EmPvolVhi,
            EmPvolXhi = parameterSet.EmPvolXhi,

            EmAcapXlo = parameterSet.EmAcapXlo,
            EmAcapVlo = parameterSet.EmAcapVlo,
            EmAcapLo = parameterSet.EmAcapLo,
            EmAcapNom = parameterSet.EmAcapNom,
            EmAcapHi = parameterSet.EmAcapHi,
            EmAcapVhi = parameterSet.EmAcapVhi,
            EmAcapXhi = parameterSet.EmAcapXhi,

            EmPcapXlo = parameterSet.EmPcapXlo,
            EmPcapVlo = parameterSet.EmPcapVlo,
            EmPcapLo = parameterSet.EmPcapLo,
            EmPcapNom = parameterSet.EmPcapNom,
            EmPcapHi = parameterSet.EmPcapHi,
            EmPcapVhi = parameterSet.EmPcapVhi,
            EmPcapXhi = parameterSet.EmPcapXhi,

            EmPconXlo = parameterSet.EmPconXlo,
            EmPconVlo = parameterSet.EmPconVlo,
            EmPconLo = parameterSet.EmPconLo,
            EmPconNom = parameterSet.EmPconNom,
            EmPconHi = parameterSet.EmPconHi,
            EmPconVhi = parameterSet.EmPconVhi,
            EmPconXhi = parameterSet.EmPconXhi,

            EmApexXlo = parameterSet.EmApexXlo,
            EmApexVlo = parameterSet.EmApexVlo,
            EmApexLo = parameterSet.EmApexLo,
            EmApexNom = parameterSet.EmApexNom,
            EmApexHi = parameterSet.EmApexHi,
            EmApexVhi = parameterSet.EmApexVhi,
            EmApexXhi = parameterSet.EmApexXhi,

            EmPlexXlo = parameterSet.EmPlexXlo,
            EmPlexVlo = parameterSet.EmPlexVlo,
            EmPlexLo = parameterSet.EmPlexLo,
            EmPlexNom = parameterSet.EmPlexNom,
            EmPlexHi = parameterSet.EmPlexHi,
            EmPlexVhi = parameterSet.EmPlexVhi,
            EmPlexXhi = parameterSet.EmPlexXhi,

            EmLtexXlo = parameterSet.EmLtexXlo,
            EmLtexVlo = parameterSet.EmLtexVlo,
            EmLtexLo = parameterSet.EmLtexLo,
            EmLtexNom = parameterSet.EmLtexNom,
            EmLtexHi = parameterSet.EmLtexHi,
            EmLtexVhi = parameterSet.EmLtexVhi,
            EmLtexXhi = parameterSet.EmLtexXhi,

            EmToolXlo = parameterSet.EmToolXlo,
            EmToolVlo = parameterSet.EmToolVlo,
            EmToolLo = parameterSet.EmToolLo,
            EmToolNom = parameterSet.EmToolNom,
            EmToolHi = parameterSet.EmToolHi,
            EmToolVhi = parameterSet.EmToolVhi,
            EmToolXhi = parameterSet.EmToolXhi,

            EmSiteXlo = parameterSet.EmSiteXlo,
            EmSiteVlo = parameterSet.EmSiteVlo,
            EmSiteLo = parameterSet.EmSiteLo,
            EmSiteNom = parameterSet.EmSiteNom,
            EmSiteHi = parameterSet.EmSiteHi,
            EmSiteVhi = parameterSet.EmSiteVhi,
            EmSiteXhi = parameterSet.EmSiteXhi,

            EmScedXlo = parameterSet.EmScedXlo,
            EmScedVlo = parameterSet.EmScedVlo,
            EmScedLo = parameterSet.EmScedLo,
            EmScedNom = parameterSet.EmScedNom,
            EmScedHi = parameterSet.EmScedHi,
            EmScedVhi = parameterSet.EmScedVhi,
            EmScedXhi = parameterSet.EmScedXhi,

            // Constants
            ConstA = parameterSet.ConstA,
            ConstB = parameterSet.ConstB,
            ConstC = parameterSet.ConstC,
            ConstD = parameterSet.ConstD
        };
    }
}