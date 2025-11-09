using Backend.Models.DTOs.CocomoThree;
using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Backend.Services.Interfaces.CocomoTwoStageOne;

namespace Backend.Services.Implementations.CocomoTwoStageOne;

/// <summary>
/// Service implementation for Estimation (COCOMO 2 Stage 1)
/// </summary>
public class EstimationCocomo2Stage1Service : IEstimationCocomo2Stage1Service
{
    private readonly IEstimationCocomo2Stage1Repository _estimationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IParameterSetCocomo2Stage1Repository _parameterSetRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICocomoCalculationCocomo2Stage1Service _calculationService;

    public EstimationCocomo2Stage1Service(
        IEstimationCocomo2Stage1Repository estimationRepository,
        IProjectRepository projectRepository,
        IParameterSetCocomo2Stage1Repository parameterSetRepository,
        ILanguageRepository languageRepository,
        ICocomoCalculationCocomo2Stage1Service calculationService)
    {
        _estimationRepository = estimationRepository;
        _projectRepository = projectRepository;
        _parameterSetRepository = parameterSetRepository;
        _languageRepository = languageRepository;
        _calculationService = calculationService;
    }

    public async Task<EstimationCocomo2Stage1Dto?> GetByIdAsync(int estimationId, int userId)
    {
        var estimation = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        if (estimation == null)
        {
            return null;
        }

        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            return null;
        }

        return MapToDto(estimation, true);
    }

    public async Task<IEnumerable<EstimationCocomo2Stage1Dto>> GetProjectEstimationsAsync(int projectId, int userId)
    {
        // Check ownership
        if (!await _projectRepository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to access this project");
        }

        var estimations = await _estimationRepository.GetByProjectIdAsync(projectId);
        return estimations.Select(e => MapToDto(e, false));
    }

    public async Task<EstimationCocomo2Stage1Dto> CreateAsync(int projectId, CreateEstimationCocomo2Stage1Dto createDto, int userId)
    {
        // Check project ownership
        if (!await _projectRepository.IsOwnerAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to create estimations in this project");
        }

        // Validate parameter set
        var paramSet = await _parameterSetRepository.GetByIdAsync(createDto.ParamSetId);
        if (paramSet == null)
        {
            throw new KeyNotFoundException($"Parameter set with ID {createDto.ParamSetId} not found");
        }

        // Verify user has access to this parameter set
        if (paramSet.UserId.HasValue && paramSet.UserId.Value != userId)
        {
            throw new UnauthorizedAccessException($"You don't have permission to use parameter set with ID {createDto.ParamSetId}");
        }

        // Validate language
        var language = await _languageRepository.GetByIdAsync(createDto.LanguageId);
        if (language == null)
        {
            throw new KeyNotFoundException($"Language with ID {createDto.LanguageId} not found");
        }

        var estimation = new EstimationCocomo2Stage1
        {
            ProjectId = projectId,
            ParamSetId = createDto.ParamSetId,
            LanguageId = createDto.LanguageId,
            EstimationName = createDto.EstimationName,
            SelectedAexp = createDto.SelectedAexp,
            SelectedPexp = createDto.SelectedPexp,
            SelectedPrec = createDto.SelectedPrec,
            SelectedRely = createDto.SelectedRely,
            SelectedTmsp = createDto.SelectedTmsp
        };

        var created = await _estimationRepository.CreateAsync(estimation);

        // Initial calculation (will be 0 since no components yet)
        await _calculationService.RecalculateEstimationAsync(created.EstimationId);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(created.EstimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<EstimationCocomo2Stage1Dto> UpdateRatingsAsync(int estimationId, UpdateRatingsCocomo2Stage1Dto updateDto, int userId)
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
        if (updateDto.SelectedAexp != null) estimation.SelectedAexp = updateDto.SelectedAexp;
        if (updateDto.SelectedPexp != null) estimation.SelectedPexp = updateDto.SelectedPexp;
        if (updateDto.SelectedPrec != null) estimation.SelectedPrec = updateDto.SelectedPrec;
        if (updateDto.SelectedRely != null) estimation.SelectedRely = updateDto.SelectedRely;
        if (updateDto.SelectedTmsp != null) estimation.SelectedTmsp = updateDto.SelectedTmsp;

        await _estimationRepository.UpdateAsync(estimation);

        // Recalculate with new ratings
        await _calculationService.RecalculateEstimationAsync(estimationId);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<EstimationCocomo2Stage1Dto> UpdateActualResultsAsync(int estimationId, UpdateActualResultsCocomo2Stage1Dto updateDto, int userId)
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
        estimation.ActualEffortHours = updateDto.ActualEffortHours;
        estimation.ActualSloc = updateDto.ActualSloc;

        await _estimationRepository.UpdateAsync(estimation);

        // Reload with details
        var estimationWithDetails = await _estimationRepository.GetByIdWithDetailsAsync(estimationId);
        return MapToDto(estimationWithDetails!, true);
    }

    public async Task<bool> DeleteAsync(int estimationId, int userId)
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

    private static EstimationCocomo2Stage1Dto MapToDto(EstimationCocomo2Stage1 estimation, bool includeDetails)
    {
        var dto = new EstimationCocomo2Stage1Dto
        {
            EstimationId = estimation.EstimationId,
            ProjectId = estimation.ProjectId,
            EstimationName = estimation.EstimationName,
            CreatedAt = estimation.CreatedAt,
            UpdatedAt = estimation.UpdatedAt,

            // Selected ratings
            SelectedAexp = estimation.SelectedAexp,
            SelectedPexp = estimation.SelectedPexp,
            SelectedPrec = estimation.SelectedPrec,
            SelectedRely = estimation.SelectedRely,
            SelectedTmsp = estimation.SelectedTmsp,

            // Calculated results
            TotalFp = estimation.TotalFp,
            Sloc = estimation.Sloc,
            Ksloc = estimation.Ksloc,
            AexpMultiplier = estimation.AexpMultiplier,
            PexpMultiplier = estimation.PexpMultiplier,
            PrecMultiplier = estimation.PrecMultiplier,
            RelyMultiplier = estimation.RelyMultiplier,
            TmspMultiplier = estimation.TmspMultiplier,
            Eaf = estimation.Eaf,
            EffortPm = estimation.EffortPm,
            EffortHours = estimation.EffortHours,

            // Actual results
            ActualEffortPm = estimation.ActualEffortPm,
            ActualEffortHours = estimation.ActualEffortHours,
            ActualSloc = estimation.ActualSloc
        };

        if (includeDetails)
        {
            // Include Language
            if (estimation.Language != null)
            {
                dto.Language = new LanguageDto
                {
                    LanguageId = estimation.Language.LanguageId,
                    Name = estimation.Language.Name,
                    SlocPerUfp = estimation.Language.SlocPerUfp
                };
            }

            // Include ParameterSet
            if (estimation.ParameterSet != null)
            {
                dto.ParameterSet = new ParameterSetCocomo2Stage1Dto
                {
                    ParamSetId = estimation.ParameterSet.ParamSetId,
                    UserId = estimation.ParameterSet.UserId,
                    SetName = estimation.ParameterSet.SetName,
                    IsDefault = estimation.ParameterSet.IsDefault,
                    CreatedAt = estimation.ParameterSet.CreatedAt,
                    ConstA = estimation.ParameterSet.ConstA,
                    ConstB = estimation.ParameterSet.ConstB,
                    
                    // AEXP
                    AexpVeryLow = estimation.ParameterSet.AexpVeryLow,
                    AexpLow = estimation.ParameterSet.AexpLow,
                    AexpNominal = estimation.ParameterSet.AexpNominal,
                    AexpHigh = estimation.ParameterSet.AexpHigh,
                    
                    // PEXP
                    PexpVeryLow = estimation.ParameterSet.PexpVeryLow,
                    PexpLow = estimation.ParameterSet.PexpLow,
                    PexpNominal = estimation.ParameterSet.PexpNominal,
                    PexpHigh = estimation.ParameterSet.PexpHigh,
                    
                    // PREC
                    PrecVeryLow = estimation.ParameterSet.PrecVeryLow,
                    PrecLow = estimation.ParameterSet.PrecLow,
                    PrecNominal = estimation.ParameterSet.PrecNominal,
                    PrecHigh = estimation.ParameterSet.PrecHigh,
                    
                    // RELY
                    RelyLow = estimation.ParameterSet.RelyLow,
                    RelyNominal = estimation.ParameterSet.RelyNominal,
                    RelyHigh = estimation.ParameterSet.RelyHigh,
                    
                    // TMSP
                    TmspLow = estimation.ParameterSet.TmspLow,
                    TmspNominal = estimation.ParameterSet.TmspNominal,
                    TmspHigh = estimation.ParameterSet.TmspHigh
                };
            }

            // Include Components
            if (estimation.Components != null && estimation.Components.Any())
            {
                dto.Components = estimation.Components.Select(c => new ComponentCocomo2Stage1Dto
                {
                    ComponentId = c.ComponentId,
                    EstimationId = c.EstimationId,
                    ComponentName = c.ComponentName,
                    Description = c.Description,
                    ComponentType = c.ComponentType,
                    SizeFp = c.SizeFp,
                    ReusePercent = c.ReusePercent,
                    ChangePercent = c.ChangePercent,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();
            }
        }

        return dto;
    }
}
