using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Backend.Services.Interfaces.CocomoTwoStageOne;

namespace Backend.Services.Implementations.CocomoTwoStageOne;

/// <summary>
/// Service implementation for Component (COCOMO 2 Stage 1)
/// </summary>
public class ComponentCocomo2Stage1Service : IComponentCocomo2Stage1Service
{
    private readonly IComponentCocomo2Stage1Repository _componentRepository;
    private readonly IEstimationCocomo2Stage1Repository _estimationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICocomoCalculationCocomo2Stage1Service _calculationService;

    public ComponentCocomo2Stage1Service(
        IComponentCocomo2Stage1Repository componentRepository,
        IEstimationCocomo2Stage1Repository estimationRepository,
        IProjectRepository projectRepository,
        ICocomoCalculationCocomo2Stage1Service calculationService)
    {
        _componentRepository = componentRepository;
        _estimationRepository = estimationRepository;
        _projectRepository = projectRepository;
        _calculationService = calculationService;
    }

    public async Task<ComponentCocomo2Stage1Dto?> GetByIdAsync(int componentId, int userId)
    {
        var component = await _componentRepository.GetByIdAsync(componentId);
        if (component == null)
        {
            return null;
        }

        // Verify ownership through estimation -> project
        await VerifyOwnershipAsync(component.EstimationId, userId);

        return MapToDto(component);
    }

    public async Task<IEnumerable<ComponentCocomo2Stage1Dto>> GetEstimationComponentsAsync(int estimationId, int userId)
    {
        // Verify ownership
        await VerifyOwnershipAsync(estimationId, userId);

        var components = await _componentRepository.GetByEstimationIdAsync(estimationId);
        return components.Select(MapToDto);
    }

    public async Task<ComponentCocomo2Stage1Dto> CreateAsync(int estimationId, CreateComponentCocomo2Stage1Dto createDto, int userId)
    {
        // Verify ownership
        await VerifyOwnershipAsync(estimationId, userId);

        var component = new EstimationComponentCocomo2Stage1
        {
            EstimationId = estimationId,
            ComponentName = createDto.ComponentName,
            Description = createDto.Description,
            ComponentType = createDto.ComponentType,
            SizeFp = createDto.SizeFp,
            ReusePercent = createDto.ReusePercent,
            ChangePercent = createDto.ChangePercent,
            Notes = createDto.Notes
        };

        var created = await _componentRepository.CreateAsync(component);

        // Recalculate estimation
        await _calculationService.RecalculateEstimationAsync(estimationId);

        return MapToDto(created);
    }

    public async Task<IEnumerable<ComponentCocomo2Stage1Dto>> CreateBatchAsync(int estimationId, CreateBatchComponentsCocomo2Stage1Dto createDto, int userId)
    {
        // Verify ownership
        await VerifyOwnershipAsync(estimationId, userId);

        var components = createDto.Components.Select(dto => new EstimationComponentCocomo2Stage1
        {
            EstimationId = estimationId,
            ComponentName = dto.ComponentName,
            Description = dto.Description,
            ComponentType = dto.ComponentType,
            SizeFp = dto.SizeFp,
            ReusePercent = dto.ReusePercent,
            ChangePercent = dto.ChangePercent,
            Notes = dto.Notes
        });

        var created = await _componentRepository.CreateBatchAsync(components);

        // Recalculate estimation once after all components are added
        await _calculationService.RecalculateEstimationAsync(estimationId);

        return created.Select(MapToDto);
    }

    public async Task<ComponentCocomo2Stage1Dto> UpdateAsync(int componentId, UpdateComponentCocomo2Stage1Dto updateDto, int userId)
    {
        var component = await _componentRepository.GetByIdAsync(componentId);
        if (component == null)
        {
            throw new KeyNotFoundException($"Component with ID {componentId} not found");
        }

        // Verify ownership
        await VerifyOwnershipAsync(component.EstimationId, userId);

        // Update properties
        component.ComponentName = updateDto.ComponentName;
        component.Description = updateDto.Description;
        component.ComponentType = updateDto.ComponentType;
        component.SizeFp = updateDto.SizeFp;
        component.ReusePercent = updateDto.ReusePercent;
        component.ChangePercent = updateDto.ChangePercent;
        component.Notes = updateDto.Notes;

        var updated = await _componentRepository.UpdateAsync(component);

        // Recalculate estimation
        await _calculationService.RecalculateEstimationAsync(component.EstimationId);

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int componentId, int userId)
    {
        var component = await _componentRepository.GetByIdAsync(componentId);
        if (component == null)
        {
            return false;
        }

        // Verify ownership
        await VerifyOwnershipAsync(component.EstimationId, userId);

        var estimationId = component.EstimationId;
        var result = await _componentRepository.DeleteAsync(componentId);

        if (result)
        {
            // Recalculate estimation after deletion
            await _calculationService.RecalculateEstimationAsync(estimationId);
        }

        return result;
    }

    private async Task VerifyOwnershipAsync(int estimationId, int userId)
    {
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to access this component");
        }
    }

    private static ComponentCocomo2Stage1Dto MapToDto(EstimationComponentCocomo2Stage1 component)
    {
        return new ComponentCocomo2Stage1Dto
        {
            ComponentId = component.ComponentId,
            EstimationId = component.EstimationId,
            ComponentName = component.ComponentName,
            Description = component.Description,
            ComponentType = component.ComponentType,
            SizeFp = component.SizeFp,
            ReusePercent = component.ReusePercent,
            ChangePercent = component.ChangePercent,
            Notes = component.Notes,
            CreatedAt = component.CreatedAt,
            UpdatedAt = component.UpdatedAt
        };
    }
}
