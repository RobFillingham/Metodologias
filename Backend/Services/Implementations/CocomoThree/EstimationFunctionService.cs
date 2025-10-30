using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Entities.CocomoThree;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Services.Implementations.CocomoThree;

/// <summary>
/// Service implementation for EstimationFunction operations
/// </summary>
public class EstimationFunctionService : IEstimationFunctionService
{
    private readonly IEstimationFunctionRepository _functionRepository;
    private readonly IEstimationRepository _estimationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICocomoCalculationService _calculationService;

    public EstimationFunctionService(
        IEstimationFunctionRepository functionRepository,
        IEstimationRepository estimationRepository,
        IProjectRepository projectRepository,
        ICocomoCalculationService calculationService)
    {
        _functionRepository = functionRepository;
        _estimationRepository = estimationRepository;
        _projectRepository = projectRepository;
        _calculationService = calculationService;
    }

    public async Task<IEnumerable<EstimationFunctionDto>> GetEstimationFunctionsAsync(int estimationId, int userId)
    {
        // Verify access through estimation -> project ownership
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to access this estimation");
        }

        var functions = await _functionRepository.GetByEstimationIdAsync(estimationId);
        
        return functions.Select(f => MapToDto(f));
    }

    public async Task<EstimationFunctionDto?> GetFunctionByIdAsync(int functionId, int userId)
    {
        var function = await _functionRepository.GetByIdAsync(functionId);
        
        if (function == null)
            return null;

        // Verify access
        var estimation = await _estimationRepository.GetByIdAsync(function.EstimationId);
        if (estimation == null || !await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            return null;
        }

        return MapToDto(function);
    }

    public async Task<EstimationFunctionDto> CreateFunctionAsync(int estimationId, CreateEstimationFunctionDto createDto, int userId)
    {
        // Verify access
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to add functions to this estimation");
        }

        var function = new EstimationFunction
        {
            EstimationId = estimationId,
            Name = createDto.Name,
            Type = createDto.Type.ToUpper(),
            Det = createDto.Det,
            RetFtr = createDto.RetFtr
        };

        // Calculate complexity and points
        await _calculationService.CalculateFunctionComplexityAsync(function);

        // Save function
        var createdFunction = await _functionRepository.CreateAsync(function);

        // Recalculate entire estimation
        await _calculationService.RecalculateEstimationAsync(estimationId);

        return MapToDto(createdFunction);
    }

    public async Task<IEnumerable<EstimationFunctionDto>> CreateBatchFunctionsAsync(int estimationId, CreateBatchEstimationFunctionsDto batchDto, int userId)
    {
        // Verify access
        var estimation = await _estimationRepository.GetByIdAsync(estimationId);
        if (estimation == null)
        {
            throw new KeyNotFoundException($"Estimation with ID {estimationId} not found");
        }

        if (!await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to add functions to this estimation");
        }

        var functions = new List<EstimationFunction>();

        foreach (var dto in batchDto.Functions)
        {
            var function = new EstimationFunction
            {
                EstimationId = estimationId,
                Name = dto.Name,
                Type = dto.Type.ToUpper(),
                Det = dto.Det,
                RetFtr = dto.RetFtr
            };

            // Calculate complexity and points for each function
            await _calculationService.CalculateFunctionComplexityAsync(function);
            functions.Add(function);
        }

        // Save all functions at once
        var createdFunctions = await _functionRepository.CreateBatchAsync(functions);

        // Recalculate estimation ONCE (optimization)
        await _calculationService.RecalculateEstimationAsync(estimationId);

        return createdFunctions.Select(f => MapToDto(f));
    }

    public async Task<EstimationFunctionDto> UpdateFunctionAsync(int functionId, UpdateEstimationFunctionDto updateDto, int userId)
    {
        var function = await _functionRepository.GetByIdAsync(functionId);
        if (function == null)
        {
            throw new KeyNotFoundException($"Function with ID {functionId} not found");
        }

        // Verify access
        var estimation = await _estimationRepository.GetByIdAsync(function.EstimationId);
        if (estimation == null || !await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this function");
        }

        // Update function data
        function.Name = updateDto.Name;
        function.Type = updateDto.Type.ToUpper();
        function.Det = updateDto.Det;
        function.RetFtr = updateDto.RetFtr;

        // Recalculate complexity and points
        await _calculationService.CalculateFunctionComplexityAsync(function);

        // Save function
        var updatedFunction = await _functionRepository.UpdateAsync(function);

        // Recalculate entire estimation
        await _calculationService.RecalculateEstimationAsync(function.EstimationId);

        return MapToDto(updatedFunction);
    }

    public async Task<bool> DeleteFunctionAsync(int functionId, int userId)
    {
        var function = await _functionRepository.GetByIdAsync(functionId);
        if (function == null)
        {
            return false;
        }

        // Verify access
        var estimation = await _estimationRepository.GetByIdAsync(function.EstimationId);
        if (estimation == null || !await _projectRepository.IsOwnerAsync(estimation.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this function");
        }

        var estimationId = function.EstimationId;

        // Delete function
        var result = await _functionRepository.DeleteAsync(functionId);

        if (result)
        {
            // Recalculate estimation after deletion
            await _calculationService.RecalculateEstimationAsync(estimationId);
        }

        return result;
    }

    private static EstimationFunctionDto MapToDto(EstimationFunction function)
    {
        return new EstimationFunctionDto
        {
            FunctionId = function.FunctionId,
            EstimationId = function.EstimationId,
            Name = function.Name,
            Type = function.Type,
            Det = function.Det,
            RetFtr = function.RetFtr,
            Complexity = function.Complexity,
            CalculatedPoints = function.CalculatedPoints
        };
    }
}
