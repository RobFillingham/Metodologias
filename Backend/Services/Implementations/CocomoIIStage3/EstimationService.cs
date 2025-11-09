using Backend.Models.DTOs.CocomoIIStage3;
using Backend.Models.Entities.CocomoIIStage3;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Backend.Services.Interfaces.CocomoIIStage3;

namespace Backend.Services.Implementations.CocomoIIStage3;

/// <summary>
/// Service implementation for Estimation operations
/// </summary>
public class EstimationService : IEstimationService
{
    private readonly IEstimationRepository _estimationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IParameterSetRepository _parameterSetRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICocomoCalculationService _calculationService;

    public EstimationService(
        IEstimationRepository estimationRepository,
        IProjectRepository projectRepository,
        IParameterSetRepository parameterSetRepository,
        ILanguageRepository languageRepository,
        ICocomoCalculationService calculationService)
    {
        _estimationRepository = estimationRepository;
        _projectRepository = projectRepository;
        _parameterSetRepository = parameterSetRepository;
        _languageRepository = languageRepository;
        _calculationService = calculationService;
    }

    public async Task<IEnumerable<EstimationDto>> GetProjectEstimationsAsync(int projectId, int userId)
    {
        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to access this project");
        }

        var estimations = await _estimationRepository.GetByProjectIdAsync(projectId);

        return estimations.Select(e => MapToDto(e, true));
    }

    public async Task<EstimationDto?> GetEstimationByIdAsync(int estimationId, int userId)
    {
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);

        if (estimation == null)
            return null;

        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to access this estimation");
        }

        return MapToDto(estimation, true);
    }

    public async Task<EstimationDto> CreateEstimationAsync(int projectId, CreateEstimationDto createDto, int userId)
    {
        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to create estimations in this project");
        }

        // Validate parameter set exists
        var paramSet = await _parameterSetRepository.GetByIdAsync(createDto.ParamSetId);
        if (paramSet == null)
        {
            throw new KeyNotFoundException($"Parameter set with ID {createDto.ParamSetId} not found");
        }

        // Verify user has access to this parameter set (either it's default or belongs to user)
        if (paramSet.UserId.HasValue && paramSet.UserId.Value != userId)
        {
            throw new UnauthorizedAccessException($"You don't have permission to use parameter set with ID {createDto.ParamSetId}");
        }

        // Validate language exists
        if (!await _languageRepository.ExistsAsync(createDto.LanguageId))
        {
            throw new KeyNotFoundException($"Language with ID {createDto.LanguageId} not found");
        }

        var estimation = new Estimation
        {
            ProjectId = projectId,
            ParamSetId = createDto.ParamSetId,
            LanguageId = createDto.LanguageId,
            EstimationName = createDto.EstimationName,
            CreatedAt = DateTime.UtcNow,

            // Selected ratings
            SelectedSfPrec = createDto.SelectedSfPrec,
            SelectedSfFlex = createDto.SelectedSfFlex,
            SelectedSfResl = createDto.SelectedSfResl,
            SelectedSfTeam = createDto.SelectedSfTeam,
            SelectedSfPmat = createDto.SelectedSfPmat,

            // Updated for 17 EM factors
            SelectedEmRely = createDto.SelectedEmRely,
            SelectedEmData = createDto.SelectedEmData,
            SelectedEmCplx = createDto.SelectedEmCplx,
            SelectedEmRuse = createDto.SelectedEmRuse,
            SelectedEmDocu = createDto.SelectedEmDocu,
            SelectedEmTime = createDto.SelectedEmTime,
            SelectedEmStor = createDto.SelectedEmStor,
            SelectedEmPvol = createDto.SelectedEmPvol,
            SelectedEmAcap = createDto.SelectedEmAcap,
            SelectedEmPcap = createDto.SelectedEmPcap,
            SelectedEmPcon = createDto.SelectedEmPcon,
            SelectedEmApex = createDto.SelectedEmApex,
            SelectedEmPlex = createDto.SelectedEmPlex,
            SelectedEmLtex = createDto.SelectedEmLtex,
            SelectedEmTool = createDto.SelectedEmTool,
            SelectedEmSite = createDto.SelectedEmSite,
            SelectedEmSced = createDto.SelectedEmSced
        };

        var createdEstimation = await _estimationRepository.CreateAsync(estimation);

        // Initial calculation (will be 0 since no functions yet)
        await _calculationService.RecalculateEstimationAsync(createdEstimation.EstimationId);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(createdEstimation.EstimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<EstimationDto> UpdateEstimationRatingsAsync(int estimationId, UpdateEstimationRatingsDto updateDto, int userId)
    {
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);

        if (estimation == null)
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");

        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this estimation");
        }

        // Update ratings
        estimation.SelectedSfPrec = updateDto.SelectedSfPrec;
        estimation.SelectedSfFlex = updateDto.SelectedSfFlex;
        estimation.SelectedSfResl = updateDto.SelectedSfResl;
        estimation.SelectedSfTeam = updateDto.SelectedSfTeam;
        estimation.SelectedSfPmat = updateDto.SelectedSfPmat;

        // Updated for 17 EM factors
        estimation.SelectedEmRely = updateDto.SelectedEmRely;
        estimation.SelectedEmData = updateDto.SelectedEmData;
        estimation.SelectedEmCplx = updateDto.SelectedEmCplx;
        estimation.SelectedEmRuse = updateDto.SelectedEmRuse;
        estimation.SelectedEmDocu = updateDto.SelectedEmDocu;
        estimation.SelectedEmTime = updateDto.SelectedEmTime;
        estimation.SelectedEmStor = updateDto.SelectedEmStor;
        estimation.SelectedEmPvol = updateDto.SelectedEmPvol;
        estimation.SelectedEmAcap = updateDto.SelectedEmAcap;
        estimation.SelectedEmPcap = updateDto.SelectedEmPcap;
        estimation.SelectedEmPcon = updateDto.SelectedEmPcon;
        estimation.SelectedEmApex = updateDto.SelectedEmApex;
        estimation.SelectedEmPlex = updateDto.SelectedEmPlex;
        estimation.SelectedEmLtex = updateDto.SelectedEmLtex;
        estimation.SelectedEmTool = updateDto.SelectedEmTool;
        estimation.SelectedEmSite = updateDto.SelectedEmSite;
        estimation.SelectedEmSced = updateDto.SelectedEmSced;

        var updatedEstimation = await _estimationRepository.UpdateAsync(estimation);

        // Recalculate
        await _calculationService.RecalculateEstimationAsync(updatedEstimation.EstimationId);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(updatedEstimation.EstimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<EstimationDto> UpdateActualResultsAsync(int estimationId, UpdateActualResultsDto updateDto, int userId)
    {
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);

        if (estimation == null)
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");

        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this estimation");
        }

        // Update actual results
        estimation.ActualEffortPm = updateDto.ActualEffortPm;
        estimation.ActualTdevMonths = updateDto.ActualTdevMonths;
        estimation.ActualSloc = updateDto.ActualSloc;

        var updatedEstimation = await _estimationRepository.UpdateAsync(estimation);

        return MapToDto(updatedEstimation, true);
    }

    public async Task<bool> DeleteEstimationAsync(int estimationId, int userId)
    {
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);

        if (estimation == null)
            return false;

        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this estimation");
        }

        return await _estimationRepository.DeleteAsync(estimationId);
    }

    private static EstimationDto MapToDto(Estimation estimation, bool includeDetails)
    {
        var dto = new EstimationDto
        {
            EstimationId = estimation.EstimationId,
            ProjectId = estimation.ProjectId,
            EstimationName = estimation.EstimationName,
            CreatedAt = estimation.CreatedAt,

            // Selected ratings
            SelectedSfPrec = estimation.SelectedSfPrec,
            SelectedSfFlex = estimation.SelectedSfFlex,
            SelectedSfResl = estimation.SelectedSfResl,
            SelectedSfTeam = estimation.SelectedSfTeam,
            SelectedSfPmat = estimation.SelectedSfPmat,

            // Updated for 17 EM factors
            SelectedEmRely = estimation.SelectedEmRely,
            SelectedEmData = estimation.SelectedEmData,
            SelectedEmCplx = estimation.SelectedEmCplx,
            SelectedEmRuse = estimation.SelectedEmRuse,
            SelectedEmDocu = estimation.SelectedEmDocu,
            SelectedEmTime = estimation.SelectedEmTime,
            SelectedEmStor = estimation.SelectedEmStor,
            SelectedEmPvol = estimation.SelectedEmPvol,
            SelectedEmAcap = estimation.SelectedEmAcap,
            SelectedEmPcap = estimation.SelectedEmPcap,
            SelectedEmPcon = estimation.SelectedEmPcon,
            SelectedEmApex = estimation.SelectedEmApex,
            SelectedEmPlex = estimation.SelectedEmPlex,
            SelectedEmLtex = estimation.SelectedEmLtex,
            SelectedEmTool = estimation.SelectedEmTool,
            SelectedEmSite = estimation.SelectedEmSite,
            SelectedEmSced = estimation.SelectedEmSced,

            // Calculated results
            TotalUfp = estimation.TotalUfp,
            Sloc = estimation.Sloc,
            Ksloc = estimation.Ksloc,
            SumSf = estimation.SumSf,
            ExponentE = estimation.ExponentE,
            Eaf = estimation.Eaf,
            EffortPm = estimation.EffortPm,
            TdevMonths = estimation.TdevMonths,
            AvgTeamSize = estimation.AvgTeamSize,

            // Actual results
            ActualEffortPm = estimation.ActualEffortPm,
            ActualTdevMonths = estimation.ActualTdevMonths,
            ActualSloc = estimation.ActualSloc
        };

        if (includeDetails)
        {
            // Include related entities
            if (estimation.Language != null)
            {
                dto.Language = new LanguageDto
                {
                    LanguageId = estimation.Language.LanguageId,
                    Name = estimation.Language.Name,
                    SlocPerUfp = estimation.Language.SlocPerUfp
                };
            }

            if (estimation.ParameterSet != null)
            {
                dto.ParameterSet = new ParameterSetDto
                {
                    ParamSetId = estimation.ParameterSet.ParamSetId,
                    SetName = estimation.ParameterSet.SetName,
                    IsDefault = estimation.ParameterSet.IsDefault,
                    UserId = estimation.ParameterSet.UserId,

                    // Scale Factors
                    SfPrecXlo = estimation.ParameterSet.SfPrecXlo,
                    SfPrecVlo = estimation.ParameterSet.SfPrecVlo,
                    SfPrecLo = estimation.ParameterSet.SfPrecLo,
                    SfPrecNom = estimation.ParameterSet.SfPrecNom,
                    SfPrecHi = estimation.ParameterSet.SfPrecHi,
                    SfPrecVhi = estimation.ParameterSet.SfPrecVhi,
                    SfPrecXhi = estimation.ParameterSet.SfPrecXhi,

                    SfFlexXlo = estimation.ParameterSet.SfFlexXlo,
                    SfFlexVlo = estimation.ParameterSet.SfFlexVlo,
                    SfFlexLo = estimation.ParameterSet.SfFlexLo,
                    SfFlexNom = estimation.ParameterSet.SfFlexNom,
                    SfFlexHi = estimation.ParameterSet.SfFlexHi,
                    SfFlexVhi = estimation.ParameterSet.SfFlexVhi,
                    SfFlexXhi = estimation.ParameterSet.SfFlexXhi,

                    SfReslXlo = estimation.ParameterSet.SfReslXlo,
                    SfReslVlo = estimation.ParameterSet.SfReslVlo,
                    SfReslLo = estimation.ParameterSet.SfReslLo,
                    SfReslNom = estimation.ParameterSet.SfReslNom,
                    SfReslHi = estimation.ParameterSet.SfReslHi,
                    SfReslVhi = estimation.ParameterSet.SfReslVhi,
                    SfReslXhi = estimation.ParameterSet.SfReslXhi,

                    SfTeamXlo = estimation.ParameterSet.SfTeamXlo,
                    SfTeamVlo = estimation.ParameterSet.SfTeamVlo,
                    SfTeamLo = estimation.ParameterSet.SfTeamLo,
                    SfTeamNom = estimation.ParameterSet.SfTeamNom,
                    SfTeamHi = estimation.ParameterSet.SfTeamHi,
                    SfTeamVhi = estimation.ParameterSet.SfTeamVhi,
                    SfTeamXhi = estimation.ParameterSet.SfTeamXhi,

                    SfPmatXlo = estimation.ParameterSet.SfPmatXlo,
                    SfPmatVlo = estimation.ParameterSet.SfPmatVlo,
                    SfPmatLo = estimation.ParameterSet.SfPmatLo,
                    SfPmatNom = estimation.ParameterSet.SfPmatNom,
                    SfPmatHi = estimation.ParameterSet.SfPmatHi,
                    SfPmatVhi = estimation.ParameterSet.SfPmatVhi,
                    SfPmatXhi = estimation.ParameterSet.SfPmatXhi,

                    // Effort Multipliers - Updated for 17 factors
                    EmRelyXlo = estimation.ParameterSet.EmRelyXlo,
                    EmRelyVlo = estimation.ParameterSet.EmRelyVlo,
                    EmRelyLo = estimation.ParameterSet.EmRelyLo,
                    EmRelyNom = estimation.ParameterSet.EmRelyNom,
                    EmRelyHi = estimation.ParameterSet.EmRelyHi,
                    EmRelyVhi = estimation.ParameterSet.EmRelyVhi,
                    EmRelyXhi = estimation.ParameterSet.EmRelyXhi,

                    EmDataXlo = estimation.ParameterSet.EmDataXlo,
                    EmDataVlo = estimation.ParameterSet.EmDataVlo,
                    EmDataLo = estimation.ParameterSet.EmDataLo,
                    EmDataNom = estimation.ParameterSet.EmDataNom,
                    EmDataHi = estimation.ParameterSet.EmDataHi,
                    EmDataVhi = estimation.ParameterSet.EmDataVhi,
                    EmDataXhi = estimation.ParameterSet.EmDataXhi,

                    EmCplxXlo = estimation.ParameterSet.EmCplxXlo,
                    EmCplxVlo = estimation.ParameterSet.EmCplxVlo,
                    EmCplxLo = estimation.ParameterSet.EmCplxLo,
                    EmCplxNom = estimation.ParameterSet.EmCplxNom,
                    EmCplxHi = estimation.ParameterSet.EmCplxHi,
                    EmCplxVhi = estimation.ParameterSet.EmCplxVhi,
                    EmCplxXhi = estimation.ParameterSet.EmCplxXhi,

                    EmRuseXlo = estimation.ParameterSet.EmRuseXlo,
                    EmRuseVlo = estimation.ParameterSet.EmRuseVlo,
                    EmRuseLo = estimation.ParameterSet.EmRuseLo,
                    EmRuseNom = estimation.ParameterSet.EmRuseNom,
                    EmRuseHi = estimation.ParameterSet.EmRuseHi,
                    EmRuseVhi = estimation.ParameterSet.EmRuseVhi,
                    EmRuseXhi = estimation.ParameterSet.EmRuseXhi,

                    EmDocuXlo = estimation.ParameterSet.EmDocuXlo,
                    EmDocuVlo = estimation.ParameterSet.EmDocuVlo,
                    EmDocuLo = estimation.ParameterSet.EmDocuLo,
                    EmDocuNom = estimation.ParameterSet.EmDocuNom,
                    EmDocuHi = estimation.ParameterSet.EmDocuHi,
                    EmDocuVhi = estimation.ParameterSet.EmDocuVhi,
                    EmDocuXhi = estimation.ParameterSet.EmDocuXhi,

                    EmTimeXlo = estimation.ParameterSet.EmTimeXlo,
                    EmTimeVlo = estimation.ParameterSet.EmTimeVlo,
                    EmTimeLo = estimation.ParameterSet.EmTimeLo,
                    EmTimeNom = estimation.ParameterSet.EmTimeNom,
                    EmTimeHi = estimation.ParameterSet.EmTimeHi,
                    EmTimeVhi = estimation.ParameterSet.EmTimeVhi,
                    EmTimeXhi = estimation.ParameterSet.EmTimeXhi,

                    EmStorXlo = estimation.ParameterSet.EmStorXlo,
                    EmStorVlo = estimation.ParameterSet.EmStorVlo,
                    EmStorLo = estimation.ParameterSet.EmStorLo,
                    EmStorNom = estimation.ParameterSet.EmStorNom,
                    EmStorHi = estimation.ParameterSet.EmStorHi,
                    EmStorVhi = estimation.ParameterSet.EmStorVhi,
                    EmStorXhi = estimation.ParameterSet.EmStorXhi,

                    EmPvolXlo = estimation.ParameterSet.EmPvolXlo,
                    EmPvolVlo = estimation.ParameterSet.EmPvolVlo,
                    EmPvolLo = estimation.ParameterSet.EmPvolLo,
                    EmPvolNom = estimation.ParameterSet.EmPvolNom,
                    EmPvolHi = estimation.ParameterSet.EmPvolHi,
                    EmPvolVhi = estimation.ParameterSet.EmPvolVhi,
                    EmPvolXhi = estimation.ParameterSet.EmPvolXhi,

                    EmAcapXlo = estimation.ParameterSet.EmAcapXlo,
                    EmAcapVlo = estimation.ParameterSet.EmAcapVlo,
                    EmAcapLo = estimation.ParameterSet.EmAcapLo,
                    EmAcapNom = estimation.ParameterSet.EmAcapNom,
                    EmAcapHi = estimation.ParameterSet.EmAcapHi,
                    EmAcapVhi = estimation.ParameterSet.EmAcapVhi,
                    EmAcapXhi = estimation.ParameterSet.EmAcapXhi,

                    EmPcapXlo = estimation.ParameterSet.EmPcapXlo,
                    EmPcapVlo = estimation.ParameterSet.EmPcapVlo,
                    EmPcapLo = estimation.ParameterSet.EmPcapLo,
                    EmPcapNom = estimation.ParameterSet.EmPcapNom,
                    EmPcapHi = estimation.ParameterSet.EmPcapHi,
                    EmPcapVhi = estimation.ParameterSet.EmPcapVhi,
                    EmPcapXhi = estimation.ParameterSet.EmPcapXhi,

                    EmPconXlo = estimation.ParameterSet.EmPconXlo,
                    EmPconVlo = estimation.ParameterSet.EmPconVlo,
                    EmPconLo = estimation.ParameterSet.EmPconLo,
                    EmPconNom = estimation.ParameterSet.EmPconNom,
                    EmPconHi = estimation.ParameterSet.EmPconHi,
                    EmPconVhi = estimation.ParameterSet.EmPconVhi,
                    EmPconXhi = estimation.ParameterSet.EmPconXhi,

                    EmApexXlo = estimation.ParameterSet.EmApexXlo,
                    EmApexVlo = estimation.ParameterSet.EmApexVlo,
                    EmApexLo = estimation.ParameterSet.EmApexLo,
                    EmApexNom = estimation.ParameterSet.EmApexNom,
                    EmApexHi = estimation.ParameterSet.EmApexHi,
                    EmApexVhi = estimation.ParameterSet.EmApexVhi,
                    EmApexXhi = estimation.ParameterSet.EmApexXhi,

                    EmPlexXlo = estimation.ParameterSet.EmPlexXlo,
                    EmPlexVlo = estimation.ParameterSet.EmPlexVlo,
                    EmPlexLo = estimation.ParameterSet.EmPlexLo,
                    EmPlexNom = estimation.ParameterSet.EmPlexNom,
                    EmPlexHi = estimation.ParameterSet.EmPlexHi,
                    EmPlexVhi = estimation.ParameterSet.EmPlexVhi,
                    EmPlexXhi = estimation.ParameterSet.EmPlexXhi,

                    EmLtexXlo = estimation.ParameterSet.EmLtexXlo,
                    EmLtexVlo = estimation.ParameterSet.EmLtexVlo,
                    EmLtexLo = estimation.ParameterSet.EmLtexLo,
                    EmLtexNom = estimation.ParameterSet.EmLtexNom,
                    EmLtexHi = estimation.ParameterSet.EmLtexHi,
                    EmLtexVhi = estimation.ParameterSet.EmLtexVhi,
                    EmLtexXhi = estimation.ParameterSet.EmLtexXhi,

                    EmToolXlo = estimation.ParameterSet.EmToolXlo,
                    EmToolVlo = estimation.ParameterSet.EmToolVlo,
                    EmToolLo = estimation.ParameterSet.EmToolLo,
                    EmToolNom = estimation.ParameterSet.EmToolNom,
                    EmToolHi = estimation.ParameterSet.EmToolHi,
                    EmToolVhi = estimation.ParameterSet.EmToolVhi,
                    EmToolXhi = estimation.ParameterSet.EmToolXhi,

                    EmSiteXlo = estimation.ParameterSet.EmSiteXlo,
                    EmSiteVlo = estimation.ParameterSet.EmSiteVlo,
                    EmSiteLo = estimation.ParameterSet.EmSiteLo,
                    EmSiteNom = estimation.ParameterSet.EmSiteNom,
                    EmSiteHi = estimation.ParameterSet.EmSiteHi,
                    EmSiteVhi = estimation.ParameterSet.EmSiteVhi,
                    EmSiteXhi = estimation.ParameterSet.EmSiteXhi,

                    EmScedXlo = estimation.ParameterSet.EmScedXlo,
                    EmScedVlo = estimation.ParameterSet.EmScedVlo,
                    EmScedLo = estimation.ParameterSet.EmScedLo,
                    EmScedNom = estimation.ParameterSet.EmScedNom,
                    EmScedHi = estimation.ParameterSet.EmScedHi,
                    EmScedVhi = estimation.ParameterSet.EmScedVhi,
                    EmScedXhi = estimation.ParameterSet.EmScedXhi,

                    // Constants
                    ConstA = estimation.ParameterSet.ConstA,
                    ConstB = estimation.ParameterSet.ConstB,
                    ConstC = estimation.ParameterSet.ConstC,
                    ConstD = estimation.ParameterSet.ConstD
                };
            }

            if (estimation.EstimationFunctions != null && estimation.EstimationFunctions.Any())
            {
                dto.Functions = estimation.EstimationFunctions.Select(f => new EstimationFunctionDto
                {
                    FunctionId = f.FunctionId,
                    EstimationId = f.EstimationId,
                    Type = f.Type,
                    Name = f.Name,
                    Det = f.Det,
                    RetFtr = f.RetFtr,
                    Complexity = f.Complexity,
                    CalculatedPoints = f.CalculatedPoints
                }).ToList();
            }
        }

        return dto;
    }
}