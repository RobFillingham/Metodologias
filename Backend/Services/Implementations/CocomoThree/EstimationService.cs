using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Services.Implementations.CocomoThree;

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

        // Check ownership through project
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            return null;
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
            
            SelectedEmPers = createDto.SelectedEmPers,
            SelectedEmRcpx = createDto.SelectedEmRcpx,
            SelectedEmPdif = createDto.SelectedEmPdif,
            SelectedEmPrex = createDto.SelectedEmPrex,
            SelectedEmRuse = createDto.SelectedEmRuse,
            SelectedEmFcil = createDto.SelectedEmFcil,
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
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this estimation");
        }

        // Update only the ratings that are provided (not null)
        if (updateDto.SelectedSfPrec != null) estimation.SelectedSfPrec = updateDto.SelectedSfPrec;
        if (updateDto.SelectedSfFlex != null) estimation.SelectedSfFlex = updateDto.SelectedSfFlex;
        if (updateDto.SelectedSfResl != null) estimation.SelectedSfResl = updateDto.SelectedSfResl;
        if (updateDto.SelectedSfTeam != null) estimation.SelectedSfTeam = updateDto.SelectedSfTeam;
        if (updateDto.SelectedSfPmat != null) estimation.SelectedSfPmat = updateDto.SelectedSfPmat;
        
        if (updateDto.SelectedEmPers != null) estimation.SelectedEmPers = updateDto.SelectedEmPers;
        if (updateDto.SelectedEmRcpx != null) estimation.SelectedEmRcpx = updateDto.SelectedEmRcpx;
        if (updateDto.SelectedEmPdif != null) estimation.SelectedEmPdif = updateDto.SelectedEmPdif;
        if (updateDto.SelectedEmPrex != null) estimation.SelectedEmPrex = updateDto.SelectedEmPrex;
        if (updateDto.SelectedEmRuse != null) estimation.SelectedEmRuse = updateDto.SelectedEmRuse;
        if (updateDto.SelectedEmFcil != null) estimation.SelectedEmFcil = updateDto.SelectedEmFcil;
        if (updateDto.SelectedEmSced != null) estimation.SelectedEmSced = updateDto.SelectedEmSced;

        await _estimationRepository.UpdateAsync(estimation);

        // Recalculate with new ratings
        await _calculationService.RecalculateEstimationAsync(estimationId);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<EstimationDto> UpdateActualResultsAsync(int estimationId, UpdateActualResultsDto updateDto, int userId)
    {
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this estimation");
        }

        // Update actual results
        estimation.ActualEffortPm = updateDto.ActualEffortPm;
        estimation.ActualTdevMonths = updateDto.ActualTdevMonths;
        estimation.ActualSloc = updateDto.ActualSloc;

        await _estimationRepository.UpdateAsync(estimation);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<bool> DeleteEstimationAsync(int estimationId, int userId)
    {
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            return false;
        }

        // Check ownership
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
            
            SelectedEmPers = estimation.SelectedEmPers,
            SelectedEmRcpx = estimation.SelectedEmRcpx,
            SelectedEmPdif = estimation.SelectedEmPdif,
            SelectedEmPrex = estimation.SelectedEmPrex,
            SelectedEmRuse = estimation.SelectedEmRuse,
            SelectedEmFcil = estimation.SelectedEmFcil,
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
                    UserId = estimation.ParameterSet.UserId,
                    SetName = estimation.ParameterSet.SetName,
                    IsDefault = estimation.ParameterSet.IsDefault,
                    
                    // COCOMO Constants
                    ConstA = estimation.ParameterSet.ConstA,
                    ConstB = estimation.ParameterSet.ConstB,
                    ConstC = estimation.ParameterSet.ConstC,
                    ConstD = estimation.ParameterSet.ConstD,
                    
                    // Scale Factors - PREC
                    SfPrecVlo = estimation.ParameterSet.SfPrecVlo,
                    SfPrecLo = estimation.ParameterSet.SfPrecLo,
                    SfPrecNom = estimation.ParameterSet.SfPrecNom,
                    SfPrecHi = estimation.ParameterSet.SfPrecHi,
                    SfPrecVhi = estimation.ParameterSet.SfPrecVhi,
                    SfPrecXhi = estimation.ParameterSet.SfPrecXhi,
                    
                    // Scale Factors - FLEX
                    SfFlexVlo = estimation.ParameterSet.SfFlexVlo,
                    SfFlexLo = estimation.ParameterSet.SfFlexLo,
                    SfFlexNom = estimation.ParameterSet.SfFlexNom,
                    SfFlexHi = estimation.ParameterSet.SfFlexHi,
                    SfFlexVhi = estimation.ParameterSet.SfFlexVhi,
                    SfFlexXhi = estimation.ParameterSet.SfFlexXhi,
                    
                    // Scale Factors - RESL
                    SfReslVlo = estimation.ParameterSet.SfReslVlo,
                    SfReslLo = estimation.ParameterSet.SfReslLo,
                    SfReslNom = estimation.ParameterSet.SfReslNom,
                    SfReslHi = estimation.ParameterSet.SfReslHi,
                    SfReslVhi = estimation.ParameterSet.SfReslVhi,
                    SfReslXhi = estimation.ParameterSet.SfReslXhi,
                    
                    // Scale Factors - TEAM
                    SfTeamVlo = estimation.ParameterSet.SfTeamVlo,
                    SfTeamLo = estimation.ParameterSet.SfTeamLo,
                    SfTeamNom = estimation.ParameterSet.SfTeamNom,
                    SfTeamHi = estimation.ParameterSet.SfTeamHi,
                    SfTeamVhi = estimation.ParameterSet.SfTeamVhi,
                    SfTeamXhi = estimation.ParameterSet.SfTeamXhi,
                    
                    // Scale Factors - PMAT
                    SfPmatVlo = estimation.ParameterSet.SfPmatVlo,
                    SfPmatLo = estimation.ParameterSet.SfPmatLo,
                    SfPmatNom = estimation.ParameterSet.SfPmatNom,
                    SfPmatHi = estimation.ParameterSet.SfPmatHi,
                    SfPmatVhi = estimation.ParameterSet.SfPmatVhi,
                    SfPmatXhi = estimation.ParameterSet.SfPmatXhi,
                    
                    // Effort Multipliers - PERS
                    EmPersXlo = estimation.ParameterSet.EmPersXlo,
                    EmPersVlo = estimation.ParameterSet.EmPersVlo,
                    EmPersLo = estimation.ParameterSet.EmPersLo,
                    EmPersNom = estimation.ParameterSet.EmPersNom,
                    EmPersHi = estimation.ParameterSet.EmPersHi,
                    EmPersVhi = estimation.ParameterSet.EmPersVhi,
                    EmPersXhi = estimation.ParameterSet.EmPersXhi,
                    
                    // Effort Multipliers - RCPX
                    EmRcpxXlo = estimation.ParameterSet.EmRcpxXlo,
                    EmRcpxVlo = estimation.ParameterSet.EmRcpxVlo,
                    EmRcpxLo = estimation.ParameterSet.EmRcpxLo,
                    EmRcpxNom = estimation.ParameterSet.EmRcpxNom,
                    EmRcpxHi = estimation.ParameterSet.EmRcpxHi,
                    EmRcpxVhi = estimation.ParameterSet.EmRcpxVhi,
                    EmRcpxXhi = estimation.ParameterSet.EmRcpxXhi,
                    
                    // Effort Multipliers - PDIF
                    EmPdifXlo = estimation.ParameterSet.EmPdifXlo,
                    EmPdifVlo = estimation.ParameterSet.EmPdifVlo,
                    EmPdifLo = estimation.ParameterSet.EmPdifLo,
                    EmPdifNom = estimation.ParameterSet.EmPdifNom,
                    EmPdifHi = estimation.ParameterSet.EmPdifHi,
                    EmPdifVhi = estimation.ParameterSet.EmPdifVhi,
                    EmPdifXhi = estimation.ParameterSet.EmPdifXhi,
                    
                    // Effort Multipliers - PREX
                    EmPrexXlo = estimation.ParameterSet.EmPrexXlo,
                    EmPrexVlo = estimation.ParameterSet.EmPrexVlo,
                    EmPrexLo = estimation.ParameterSet.EmPrexLo,
                    EmPrexNom = estimation.ParameterSet.EmPrexNom,
                    EmPrexHi = estimation.ParameterSet.EmPrexHi,
                    EmPrexVhi = estimation.ParameterSet.EmPrexVhi,
                    EmPrexXhi = estimation.ParameterSet.EmPrexXhi,
                    
                    // Effort Multipliers - RUSE
                    EmRuseXlo = estimation.ParameterSet.EmRuseXlo,
                    EmRuseVlo = estimation.ParameterSet.EmRuseVlo,
                    EmRuseLo = estimation.ParameterSet.EmRuseLo,
                    EmRuseNom = estimation.ParameterSet.EmRuseNom,
                    EmRuseHi = estimation.ParameterSet.EmRuseHi,
                    EmRuseVhi = estimation.ParameterSet.EmRuseVhi,
                    EmRuseXhi = estimation.ParameterSet.EmRuseXhi,
                    
                    // Effort Multipliers - FCIL
                    EmFcilXlo = estimation.ParameterSet.EmFcilXlo,
                    EmFcilVlo = estimation.ParameterSet.EmFcilVlo,
                    EmFcilLo = estimation.ParameterSet.EmFcilLo,
                    EmFcilNom = estimation.ParameterSet.EmFcilNom,
                    EmFcilHi = estimation.ParameterSet.EmFcilHi,
                    EmFcilVhi = estimation.ParameterSet.EmFcilVhi,
                    EmFcilXhi = estimation.ParameterSet.EmFcilXhi,
                    
                    // Effort Multipliers - SCED
                    EmScedXlo = estimation.ParameterSet.EmScedXlo,
                    EmScedVlo = estimation.ParameterSet.EmScedVlo,
                    EmScedLo = estimation.ParameterSet.EmScedLo,
                    EmScedNom = estimation.ParameterSet.EmScedNom,
                    EmScedHi = estimation.ParameterSet.EmScedHi,
                    EmScedVhi = estimation.ParameterSet.EmScedVhi,
                    EmScedXhi = estimation.ParameterSet.EmScedXhi
                };
            }

            // Include functions
            if (estimation.EstimationFunctions != null && estimation.EstimationFunctions.Any())
            {
                dto.Functions = estimation.EstimationFunctions.Select(f => new EstimationFunctionDto
                {
                    FunctionId = f.FunctionId,
                    EstimationId = f.EstimationId,
                    Name = f.Name,
                    Type = f.Type,
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
