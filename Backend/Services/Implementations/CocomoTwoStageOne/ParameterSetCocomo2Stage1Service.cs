using Backend.Models.DTOs.CocomoTwoStageOne;
using Backend.Models.Entities.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Backend.Services.Interfaces.CocomoTwoStageOne;

namespace Backend.Services.Implementations.CocomoTwoStageOne;

/// <summary>
/// Service implementation for ParameterSet (COCOMO 2 Stage 1)
/// </summary>
public class ParameterSetCocomo2Stage1Service : IParameterSetCocomo2Stage1Service
{
    private readonly IParameterSetCocomo2Stage1Repository _repository;

    public ParameterSetCocomo2Stage1Service(IParameterSetCocomo2Stage1Repository repository)
    {
        _repository = repository;
    }

    public async Task<ParameterSetCocomo2Stage1Dto?> GetByIdAsync(int paramSetId, int userId)
    {
        var paramSet = await _repository.GetByIdAsync(paramSetId);
        if (paramSet == null)
            return null;

        // Check if it's a default set or belongs to the user
        if (!paramSet.IsDefault && paramSet.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to access this parameter set");
        }

        return MapToDto(paramSet);
    }

    public async Task<IEnumerable<ParameterSetCocomo2Stage1Dto>> GetUserSetsAsync(int userId)
    {
        var paramSets = await _repository.GetByUserIdAsync(userId);
        return paramSets.Select(MapToDto);
    }

    public async Task<IEnumerable<ParameterSetCocomo2Stage1Dto>> GetDefaultSetsAsync()
    {
        var paramSets = await _repository.GetDefaultSetsAsync();
        return paramSets.Select(MapToDto);
    }

    public async Task<ParameterSetCocomo2Stage1Dto> CreateAsync(CreateParameterSetCocomo2Stage1Dto createDto, int userId)
    {
        var paramSet = new ParameterSetCocomo2Stage1
        {
            UserId = userId,
            SetName = createDto.SetName,
            IsDefault = false,
            ConstA = createDto.ConstA,
            ConstB = createDto.ConstB,
            
            // AEXP
            AexpVeryLow = createDto.AexpVeryLow,
            AexpLow = createDto.AexpLow,
            AexpNominal = createDto.AexpNominal,
            AexpHigh = createDto.AexpHigh,
            
            // PEXP
            PexpVeryLow = createDto.PexpVeryLow,
            PexpLow = createDto.PexpLow,
            PexpNominal = createDto.PexpNominal,
            PexpHigh = createDto.PexpHigh,
            
            // PREC
            PrecVeryLow = createDto.PrecVeryLow,
            PrecLow = createDto.PrecLow,
            PrecNominal = createDto.PrecNominal,
            PrecHigh = createDto.PrecHigh,
            
            // RELY
            RelyLow = createDto.RelyLow,
            RelyNominal = createDto.RelyNominal,
            RelyHigh = createDto.RelyHigh,
            
            // TMSP
            TmspLow = createDto.TmspLow,
            TmspNominal = createDto.TmspNominal,
            TmspHigh = createDto.TmspHigh
        };

        var created = await _repository.CreateAsync(paramSet);
        return MapToDto(created);
    }

    public async Task<ParameterSetCocomo2Stage1Dto> UpdateAsync(int paramSetId, CreateParameterSetCocomo2Stage1Dto updateDto, int userId)
    {
        var paramSet = await _repository.GetByIdAsync(paramSetId);
        if (paramSet == null)
        {
            throw new KeyNotFoundException($"Parameter set with ID {paramSetId} not found");
        }

        // Check ownership
        if (paramSet.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this parameter set");
        }

        // Cannot update default sets
        if (paramSet.IsDefault)
        {
            throw new InvalidOperationException("Cannot update default parameter sets");
        }

        // Update properties
        paramSet.SetName = updateDto.SetName;
        paramSet.ConstA = updateDto.ConstA;
        paramSet.ConstB = updateDto.ConstB;
        
        // AEXP
        paramSet.AexpVeryLow = updateDto.AexpVeryLow;
        paramSet.AexpLow = updateDto.AexpLow;
        paramSet.AexpNominal = updateDto.AexpNominal;
        paramSet.AexpHigh = updateDto.AexpHigh;
        
        // PEXP
        paramSet.PexpVeryLow = updateDto.PexpVeryLow;
        paramSet.PexpLow = updateDto.PexpLow;
        paramSet.PexpNominal = updateDto.PexpNominal;
        paramSet.PexpHigh = updateDto.PexpHigh;
        
        // PREC
        paramSet.PrecVeryLow = updateDto.PrecVeryLow;
        paramSet.PrecLow = updateDto.PrecLow;
        paramSet.PrecNominal = updateDto.PrecNominal;
        paramSet.PrecHigh = updateDto.PrecHigh;
        
        // RELY
        paramSet.RelyLow = updateDto.RelyLow;
        paramSet.RelyNominal = updateDto.RelyNominal;
        paramSet.RelyHigh = updateDto.RelyHigh;
        
        // TMSP
        paramSet.TmspLow = updateDto.TmspLow;
        paramSet.TmspNominal = updateDto.TmspNominal;
        paramSet.TmspHigh = updateDto.TmspHigh;

        var updated = await _repository.UpdateAsync(paramSet);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int paramSetId, int userId)
    {
        var paramSet = await _repository.GetByIdAsync(paramSetId);
        if (paramSet == null)
        {
            return false;
        }

        // Check ownership
        if (paramSet.UserId != userId)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this parameter set");
        }

        // Cannot delete default sets
        if (paramSet.IsDefault)
        {
            throw new InvalidOperationException("Cannot delete default parameter sets");
        }

        return await _repository.DeleteAsync(paramSetId);
    }

    private static ParameterSetCocomo2Stage1Dto MapToDto(ParameterSetCocomo2Stage1 paramSet)
    {
        return new ParameterSetCocomo2Stage1Dto
        {
            ParamSetId = paramSet.ParamSetId,
            UserId = paramSet.UserId,
            SetName = paramSet.SetName,
            IsDefault = paramSet.IsDefault,
            CreatedAt = paramSet.CreatedAt,
            ConstA = paramSet.ConstA,
            ConstB = paramSet.ConstB,
            
            // AEXP
            AexpVeryLow = paramSet.AexpVeryLow,
            AexpLow = paramSet.AexpLow,
            AexpNominal = paramSet.AexpNominal,
            AexpHigh = paramSet.AexpHigh,
            
            // PEXP
            PexpVeryLow = paramSet.PexpVeryLow,
            PexpLow = paramSet.PexpLow,
            PexpNominal = paramSet.PexpNominal,
            PexpHigh = paramSet.PexpHigh,
            
            // PREC
            PrecVeryLow = paramSet.PrecVeryLow,
            PrecLow = paramSet.PrecLow,
            PrecNominal = paramSet.PrecNominal,
            PrecHigh = paramSet.PrecHigh,
            
            // RELY
            RelyLow = paramSet.RelyLow,
            RelyNominal = paramSet.RelyNominal,
            RelyHigh = paramSet.RelyHigh,
            
            // TMSP
            TmspLow = paramSet.TmspLow,
            TmspNominal = paramSet.TmspNominal,
            TmspHigh = paramSet.TmspHigh
        };
    }
}
